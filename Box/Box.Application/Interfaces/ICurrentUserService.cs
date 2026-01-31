public interface ICurrentUserService
{
    Guid UserId { get; }
    Guid Jti {get; }
    string Name { get; }
    bool IsAuthenticated { get; }
}
