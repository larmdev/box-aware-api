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
    private readonly PasswordHasher _passwordHasher;

    public AuthService(
        IConfiguration config,
        IAuthRepository repo,
        ICurrentUserService currentUser,
        ISessionService sessionService,
        PasswordHasher passwordHasher
        )
    {
        _config = config;
        _repo = repo;
        _currentUser = currentUser;
        _sessionService = sessionService;
        _passwordHasher = passwordHasher;
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

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, user.Username),
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

            await _sessionService.CreateSessionAsync(
                Guid.Parse(userId),
                jti,
                TimeSpan.FromMinutes(expireMinutes)
            );

            var response = new AuthResponseDto()
            {
                AccessToken = accessToken,
                ExpiresAt = DateTime.Now
            };

            return ApiResponse<AuthResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<AuthResponseDto>.Error(ex.Message);
        }
    }

    public async Task<ApiResponse<string>> LogOutAsync()
    {
        try
        {
            Guid userId = _currentUser.UserId;
            Guid jti = _currentUser.Jti;

            await _sessionService.DeleteSessionAsync(
                userId,
                jti.ToString()
            );

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

}
