using StackExchange.Redis;
using Box.Application.Interfaces;

public class RedisSessionService : ISessionService
{
    private readonly IDatabase _db;

    public RedisSessionService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    private static string Key(Guid userId, string jti)
        => $"session:{userId}:{jti}";

    public async Task CreateSessionAsync(Guid userId, string jti, TimeSpan ttl)
    {
        await _db.StringSetAsync(
            Key(userId, jti),
            "1",
            ttl
        );
    }

    public async Task<bool> IsSessionValidAsync(Guid userId, string jti)
    {
        return await _db.KeyExistsAsync(Key(userId, jti));
    }

    public async Task DeleteSessionAsync(Guid userId, string jti)
    {
        await _db.KeyDeleteAsync(Key(userId, jti));
    }

}
