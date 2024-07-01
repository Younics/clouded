namespace Clouded.Auth.Shared.Token.Output;

public class TokenOutput
{
    public required string Token { get; init; }
    public required DateTime Expires { get; init; }
}
