using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text.Json;

public class RedisRefreshTokenService : IRefreshTokenService
{
    private readonly IDatabase _db;
    private static readonly TimeSpan RefreshTtl = TimeSpan.FromDays(14);

    public RedisRefreshTokenService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    private static string TokenKey(string token)
        => $"refresh:{token}";

    private static string UserIndexKey(Guid userId)
        => $"user-refresh:{userId}";

    public async Task<string> CreateAsync(Guid userId, string jti, TimeSpan refreshTtl)
    {
        var refreshToken = GenerateSecureToken();

        var payload = new RefreshTokenPayload
        {
            UserId = userId,
            Jti = jti
        };

        var json = JsonSerializer.Serialize(payload);

        await _db.StringSetAsync(
            TokenKey(refreshToken),
            json,
            RefreshTtl
        );

        // index ต่อ user (ใช้ revoke all)
        await _db.SetAddAsync(
            UserIndexKey(userId),
            refreshToken
        );

        return refreshToken;
    }

    public async Task<RefreshTokenPayload?> ValidateAsync(string refreshToken)
    {
        var value = await _db.StringGetAsync(TokenKey(refreshToken));
        if (value.IsNullOrEmpty) return null;

        return JsonSerializer.Deserialize<RefreshTokenPayload>(value!);
    }

    public async Task<string> RotateAsync(string refreshToken, string newJti, TimeSpan refreshTtl)
    {
        var payload = await ValidateAsync(refreshToken);
        if (payload == null)
            throw new SecurityTokenException("Invalid refresh token");

        // revoke old
        await RevokeAsync(refreshToken);

        // issue new
        return await CreateAsync(payload.UserId, newJti, refreshTtl);
    }

    public async Task RevokeAsync(string refreshToken)
    {
        var payload = await ValidateAsync(refreshToken);
        if (payload == null) return;

        await _db.KeyDeleteAsync(TokenKey(refreshToken));
        await _db.SetRemoveAsync(
            UserIndexKey(payload.UserId),
            refreshToken
        );
    }

    public async Task RevokeAllAsync(Guid userId)
    {
        var key = UserIndexKey(userId);
        var tokens = await _db.SetMembersAsync(key);

        foreach (var token in tokens)
        {
            await _db.KeyDeleteAsync(TokenKey(token!));
        }

        await _db.KeyDeleteAsync(key);
    }

    private static string GenerateSecureToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "");
    }
}
