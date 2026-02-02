using System.Security.Claims;
using Box.Shared.Auth.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Box.Shared.Auth.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User =>
        _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated == true;

    public Guid? UserIdOrNull
    {
        get
        {
            if (!IsAuthenticated)
                return null;

            var raw = GetUserIdRaw();
            if (raw == null)
                return null;

            if (Guid.TryParse(raw, out var id))
                return id;

            return null;
        }
    }

    public Guid UserIdRequired
    {
        get
        {
            if (!IsAuthenticated)
                throw new UnauthenticatedUserException();

            var raw = GetUserIdRaw();
            if (raw == null)
                throw new MissingUserIdClaimException();

            if (!Guid.TryParse(raw, out var id))
                throw new InvalidUserIdClaimException(raw);

            return id;
        }
    }

    private string? GetUserIdRaw()
    {
        return
            User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User?.FindFirst("sub")?.Value
            ?? User?.FindFirst("userId")?.Value;
    }
}
