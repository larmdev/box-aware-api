using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly ISessionService _sessionService;

    public AuthController(
        IConfiguration config,
        ISessionService sessionService
        )
    {
        _config = config;
        _sessionService = sessionService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login()
    {
        string userId = "2530ad87-f64c-4d25-8cfe-20f4443ddb57";
        var jti = Guid.NewGuid().ToString();

        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, userId),
        new Claim(ClaimTypes.Name, "box-user"),
        new Claim(ClaimTypes.Role, "Admin"),
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

        await _sessionService.CreateSessionAsync(
            Guid.Parse(userId),
            jti,
            TimeSpan.FromMinutes(expireMinutes)
        );

        return Ok(new
        {
            access_token = new JwtSecurityTokenHandler().WriteToken(token),
            expires_at = expires
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var jti = User.FindFirstValue(JwtRegisteredClaimNames.Jti);

        if (userIdStr == null || jti == null)
            return Unauthorized();

        await _sessionService.DeleteSessionAsync(
            Guid.Parse(userIdStr),
            jti
        );

        return Ok();
    }


}

