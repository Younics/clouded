namespace Clouded.Auth.Shared.Token.Output;

public class OAuthValidateOutput
{
    /// <summary>
    /// Validated token identities
    /// </summary>
    public required object[] Identities { get; init; }

    /// <summary>
    ///
    /// </summary>
    public required object? UserId { get; init; }

    /// <summary>
    ///
    /// </summary>
    public required object? MachineId { get; init; }

    /// <summary>
    /// Validated token expires at in UTC
    /// </summary>
    public required DateTime ExpiresAt { get; init; }
}
