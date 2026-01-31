using System.Security.Claims;
using Box.Application.Interfaces;

public class CurrentUserService : ICurrentUserService
{
    public Guid UserId { get; } = Guid.NewGuid();
    public string Name { get; } = string.Empty;
    public bool IsAuthenticated { get; }

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

        
    }
}
