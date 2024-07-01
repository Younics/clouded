using System.ComponentModel.DataAnnotations;

namespace Clouded.Auth.Shared.Token.Input.Base;

public class OAuthMachineKeysInput
{
    /// <summary>
    /// Machine api key from which the request has been called.
    /// </summary>
    [Required]
    public string? ApiKey { get; set; }

    /// <summary>
    /// Machine secret key from which the request has been called.
    /// </summary>
    [Required]
    public string? SecretKey { get; set; }
}
