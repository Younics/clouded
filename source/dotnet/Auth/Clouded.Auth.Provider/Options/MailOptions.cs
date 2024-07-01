using System.ComponentModel.DataAnnotations;

namespace Clouded.Auth.Provider.Options;

public class MailOptions
{
    [Required]
    public MailDraftOptions Drafts { get; set; } = new();
}
