using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("login")]
    public IActionResult Login()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "box-user"),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var jwt = _config.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Convert.FromBase64String(jwt["Secret"]!));

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                int.Parse(jwt["ExpireMinutes"]!)
            ),
            signingCredentials: new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256
            )
        );

        return Ok(new
        {
            access_token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }
}

