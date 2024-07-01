namespace Clouded.Auth.Shared.Token.Input;

public class AppleLoginResponse
{
    public required string? Code { get; set; }
    public required string? State { get; set; }
    public required AppleUser? User { get; set; }
}

public class AppleUser
{
    public required string? Email { get; set; }
    public required AppleUserName? Name { get; set; }
}

public class AppleUserName
{
    public required string? FirstName { get; set; }
    public required string? LastName { get; set; }
}
