public interface ISessionService
{
    Task CreateSessionAsync(Guid userId, string jti, TimeSpan ttl);
    Task<bool> IsSessionValidAsync(Guid userId, string jti);
    Task DeleteSessionAsync(Guid userId, string jti);
}
