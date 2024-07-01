using System.ComponentModel.DataAnnotations;
using Clouded.Auth.Shared.Token.Input.Base;

namespace Clouded.Auth.Shared.Token.Input;

public class OAuthTokenRefreshInput : OAuthMachineKeysInput
{
    /// <summary>
    /// Access token
    /// </summary>
    [Required]
    public required string AccessToken { get; set; }

    /// <summary>
    /// Refresh token
    /// </summary>
    [Required]
    public required string RefreshToken { get; set; }
}
