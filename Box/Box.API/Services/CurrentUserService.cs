using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Box.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;

public class CurrentUserService : ICurrentUserService
{
    public Guid UserId { get; } = Guid.NewGuid();
    public Guid Jti { get; } = Guid.NewGuid();
    public string Name { get; } = string.Empty;
    public bool IsAuthenticated { get; } = false;

    public CurrentUserService(IHttpContextAccessor accessor)
    {
        var user = accessor.HttpContext?.User;

        IsAuthenticated = user?.Identity?.IsAuthenticated == true;

        var name = user?.FindFirstValue(ClaimTypes.Name);

        if (name != null)
            Name = name;

        var userIdStr = user?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdStr != null)
            UserId = Guid.Parse(userIdStr);

        var jtiStr = user?.FindFirstValue(JwtRegisteredClaimNames.Jti);
        if (jtiStr != null)
            Jti = Guid.Parse(jtiStr);
                    
    }
}
