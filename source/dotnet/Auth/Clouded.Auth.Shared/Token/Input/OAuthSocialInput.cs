using System.ComponentModel.DataAnnotations;
using Clouded.Auth.Shared.Token.Input.Base;

namespace Clouded.Auth.Shared.Token.Input;

public class OAuthSocialInput : OAuthMachineKeysInput
{
    /// <summary>
    /// Social access code
    /// </summary>
    [Required]
    public string Code { get; set; } = null!;

    /// <summary>
    /// </summary>
    [Required]
    public object UserId { get; set; } = null!;
}
