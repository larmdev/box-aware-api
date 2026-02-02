namespace Box.Shared.Auth.Constants;

public static class SessionKeys
{
    public static string UserSession(Guid userId)
        => $"session:user:{userId}";
}
