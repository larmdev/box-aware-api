namespace Box.Shared.Auth.Interfaces;

public interface ISessionService
{
    Task<bool> IsSessionValidAsync(Guid userId, string jti);
    Task SetSessionAsync(Guid userId, string jti, TimeSpan expiry);
    Task RevokeSessionAsync(Guid userId);
}
