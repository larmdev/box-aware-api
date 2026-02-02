public interface ICurrentUserService
{
    bool IsAuthenticated { get; }
    Guid? UserIdOrNull { get; }
    Guid UserIdRequired { get; }
}
