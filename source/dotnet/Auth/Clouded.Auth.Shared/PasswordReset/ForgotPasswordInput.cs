using System.ComponentModel.DataAnnotations;

namespace Clouded.Auth.Shared.PasswordReset;

public class ForgotPasswordInput
{
    /// <summary>
    /// Identity of user
    /// </summary>
    [Required]
    public required string Identity { get; set; }
}
