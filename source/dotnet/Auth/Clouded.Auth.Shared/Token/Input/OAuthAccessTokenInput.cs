using System.ComponentModel.DataAnnotations;

namespace Clouded.Auth.Shared.Token.Input;

public class OAuthAccessTokenInput
{
    /// <summary>
    /// Access token
    /// </summary>
    [Required]
    public required string AccessToken { get; set; }
}
