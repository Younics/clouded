using System.ComponentModel.DataAnnotations;

namespace Clouded.Auth.Shared.Token.Input;

public class OAuthTokenMachineInput
{
    /// <summary>
    /// API key of machine/client
    /// </summary>
    [Required]
    public required string ApiKey { get; set; }

    /// <summary>
    /// API secret key of machine/client
    /// </summary>
    [Required]
    public required string SecretKey { get; set; }
}
