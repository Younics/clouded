using System.ComponentModel.DataAnnotations;

namespace Clouded.Auth.Shared.PasswordReset;

public class PasswordResetInput
{
    /// <summary>
    /// Reset token received with email
    /// </summary>
    [Required]
    public required string ResetToken { get; set; }

    /// <summary>
    /// New password
    /// </summary>
    [Required]
    public required string Password { get; set; }
}
