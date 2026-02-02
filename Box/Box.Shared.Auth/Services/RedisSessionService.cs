using Box.Shared.Auth.Constants;
using Box.Shared.Auth.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Box.Shared.Auth.Services;

public class RedisSessionService : ISessionService
{
    private readonly IDistributedCache _cache;

    public RedisSessionService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<bool> IsSessionValidAsync(Guid userId, string jti)
    {
        var key = SessionKeys.UserSession(userId);
        var storedJti = await _cache.GetStringAsync(key);

        return !string.IsNullOrEmpty(storedJti) && storedJti == jti;
    }

    public async Task SetSessionAsync(Guid userId, string jti, TimeSpan expiry)
    {
        var key = SessionKeys.UserSession(userId);

        await _cache.SetStringAsync(
            key,
            jti,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry
            });
    }

    public async Task RevokeSessionAsync(Guid userId)
    {
        var key = SessionKeys.UserSession(userId);
        await _cache.RemoveAsync(key);
    }
}
