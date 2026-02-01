using Hangfire;

public class EmailJobService : IEmailJobService
{
    private readonly ICurrentUserService _currentUser;
    public EmailJobService(
        ICurrentUserService currentUser
    )
    {
        _currentUser = currentUser;
    }
    
    [Queue("send-email")]
    public async Task SendWelcomeEmailAsync(string msg)
    {
        // business logic
        Guid userId = _currentUser.UserId;
        return;
    }
}
