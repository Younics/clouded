namespace Clouded.Auth.Shared.Token.Output;

public class OAuthOutput
{
    /// <summary>
    /// Access token
    /// </summary>
    public required TokenOutput AccessToken { get; init; }

    /// <summary>
    /// Refresh token
    /// </summary>
    public required TokenOutput RefreshToken { get; init; }
}
