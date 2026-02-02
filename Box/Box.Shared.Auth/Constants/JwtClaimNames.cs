namespace Box.Shared.Auth.Constants;

public static class JwtClaimNames
{
    public const string UserId = System.Security.Claims.ClaimTypes.NameIdentifier;
    public const string Jti = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti;
}
