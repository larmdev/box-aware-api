using Box.Application.Interfaces;
using Box.Domain.Entities;
using Box.Application.Dtos;
using Box.Application.Common;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace Box.Application.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;
    private readonly IAuthRepository _repo;
    private readonly ICurrentUserService _currentUser;
    private readonly ISessionService _sessionService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly PasswordHasher _passwordHasher;

    private readonly int _expireMinutes;
    private readonly int _expireDays;

    public AuthService(
        IConfiguration config,
        IAuthRepository repo,
        ICurrentUserService currentUser,
        ISessionService sessionService,
        IRefreshTokenService refreshTokenService,
        PasswordHasher passwordHasher
        )
    {
        _config = config;
        _repo = repo;
        _currentUser = currentUser;
        _sessionService = sessionService;
        _refreshTokenService = refreshTokenService;
        _passwordHasher = passwordHasher;

        _expireMinutes = config.GetValue<int>("JwtSettings:ExpireMinutes");
        _expireDays = config.GetValue<int>("JwtSettings:ExpireDays");
    }

    public async Task<ApiResponse<AuthResponseDto>> LogInAsync(AuthRequestDto req)
    {
        try
        {
            User? user = await _repo.GetUserLoginAsync(req.Username);

            if (user == null) return ApiResponse<AuthResponseDto>.Error(500, "Username is not found!");

            var verifyPassword = _passwordHasher.VerifyPassword(req.Password, user.PasswordHash, user.PasswordSalt);
            if (!verifyPassword) return ApiResponse<AuthResponseDto>.Error(500, "Username or password is incorrect.");

            string userId = user.UserId.ToString();
            var jti = Guid.NewGuid().ToString();

            var (accessToken, expires) = GenerateJwt(userId, jti);

            await _sessionService.CreateSessionAsync(
                Guid.Parse(userId),
                jti,
                TimeSpan.FromMinutes(_expireMinutes)
            );

            var refreshToken = await _refreshTokenService.CreateAsync(user.UserId, jti, TimeSpan.FromDays(_expireDays));

            var response = new AuthResponseDto()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.Now
            };

            return ApiResponse<AuthResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<AuthResponseDto>.Error(ex.Message);
        }
    }

    public async Task<ApiResponse<string>> LogOutAsync(string refreshToken)
    {
        try
        {
            Guid userId = _currentUser.UserId;
            Guid jti = _currentUser.Jti;

            await _sessionService.DeleteSessionAsync(
                userId,
                jti.ToString()
            );

            await _refreshTokenService.RevokeAsync(refreshToken);

            return ApiResponse<string>.Success();
        }
        catch (Exception ex)
        {
            return ApiResponse<string>.Error(ex.Message);
        }

    }

    public async Task<ApiResponse<string>> RegisterAsync(AuthRequestDto req)
    {
        try
        {
            User? user = await _repo.GetUserLoginAsync(req.Username);

            if (user != null) return ApiResponse<string>.Error(500, "Username is duplicate");

            var passwordHash = _passwordHasher.EncryptPassword(req.Password, out string salt);

            await _repo.AddUserAsync(new User
            {
                Username = req.Username,
                PasswordHash = passwordHash,
                PasswordSalt = salt,
            });

            return ApiResponse<string>.Success();
        }
        catch (Exception ex)
        {
            return ApiResponse<string>.Error(ex.Message);
        }
    }

    public async Task<ApiResponse<AuthResponseDto>> RefreshAsync(string refreshToken)
    {
        try
        {
            var payload = await _refreshTokenService.ValidateAsync(refreshToken);
            if (payload == null)
                return ApiResponse<AuthResponseDto>.Error(401, "Invalid refresh token");

            // check session เดิม (optional แต่แนะนำ)
            var sessionValid = await _sessionService.IsSessionValidAsync(
                payload.UserId,
                payload.Jti
            );

            if (!sessionValid)
                return ApiResponse<AuthResponseDto>.Error(401, "Session expired");

            // generate new JTI + AccessToken
            var newJti = Guid.NewGuid().ToString();
            var (accessToken, expires) = GenerateJwt(payload.UserId.ToString(), newJti);

            await _sessionService.CreateSessionAsync(
                payload.UserId,
                newJti,
                TimeSpan.FromMinutes(_expireMinutes)
            );

            var newRefreshToken = await _refreshTokenService.RotateAsync(
                refreshToken,
                newJti,
                TimeSpan.FromDays(_expireDays)
            );

            var response = new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = expires
            };

            return ApiResponse<AuthResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<AuthResponseDto>.Error(ex.Message);
        }
    }


    public (string accessToken, DateTime expires) GenerateJwt(string userId, string jti)
    {
        var claims = new[]{
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            };

        var jwt = _config.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(
            Convert.FromBase64String(jwt["Secret"]!)
        );

        var expireMinutes = int.Parse(jwt["ExpireMinutes"]!);
        var expires = DateTime.UtcNow.AddMinutes(expireMinutes);

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256
            )
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return (accessToken, expires);
    }

}
