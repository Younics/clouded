using System.ComponentModel.DataAnnotations;

namespace Clouded.Auth.Provider.Options;

public class MailDraftOptions
{
    [Required]
    public MailDraftItemOptions PasswordReset { get; set; } = new();
}
