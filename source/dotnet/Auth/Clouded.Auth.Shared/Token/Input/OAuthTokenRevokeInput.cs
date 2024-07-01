using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Clouded.Auth.Shared.Token.Input;

public class OAuthTokenRevokeInput
{
    /// <summary>
    /// The refresh token
    /// </summary>
    [Required]
    public required string RefreshToken { get; set; }

    /// <summary>
    /// All tokens invalidation toggle
    /// </summary>
    /// <remarks>
    /// If this toggle is enabled, then it invalidates not only the specific token, but all other tokens based on the same authorization grant.
    /// </remarks>
    [DefaultValue(false)]
    public required bool AllOfUser { get; set; }
}
