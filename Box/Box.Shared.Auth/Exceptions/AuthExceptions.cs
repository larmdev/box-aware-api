namespace Box.Shared.Auth;

public class UnauthenticatedUserException : Exception { }

public class MissingUserIdClaimException : Exception { }

public class InvalidUserIdClaimException : Exception
{
    public InvalidUserIdClaimException(string raw)
        : base($"Invalid user id claim: {raw}") { }
}
