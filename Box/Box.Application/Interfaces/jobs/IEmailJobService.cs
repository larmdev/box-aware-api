public interface IEmailJobService
{
    Task SendWelcomeEmailAsync(string msg = "");
}
