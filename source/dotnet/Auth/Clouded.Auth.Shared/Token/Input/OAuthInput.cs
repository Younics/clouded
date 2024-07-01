using System.ComponentModel.DataAnnotations;
using Clouded.Auth.Shared.Token.Input.Base;

namespace Clouded.Auth.Shared.Token.Input;

public class OAuthInput : OAuthMachineKeysInput
{
    /// <summary>
    /// Authentication entity identity (email/nickname)
    /// </summary>
    [Required]
    public string? Identity { get; set; }

    /// <summary>
    /// Authentication entity password
    /// </summary>
    [Required]
    public string? Password { get; set; }
}
