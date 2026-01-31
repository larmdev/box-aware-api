public interface IRefreshTokenService
{
    Task<string> CreateAsync(Guid userId, string jti, TimeSpan timeSpan);

    Task<RefreshTokenPayload?> ValidateAsync(string refreshToken);

    Task<string> RotateAsync(string refreshToken, string newJti, TimeSpan timeSpan);

    Task RevokeAsync(string refreshToken);

    Task RevokeAllAsync(Guid userId);
}

public class RefreshTokenPayload
{
    public Guid UserId { get; set; }
    public string Jti { get; set; } = default!;
}
