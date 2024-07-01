using System.ComponentModel.DataAnnotations;

namespace Clouded.Auth.Provider.Options;

public class MailDraftItemOptions
{
    public string? From { get; set; }

    [Required]
    public string Subject { get; set; } = null!;

    [Required]
    public string Template { get; set; } = null!;

    [Required]
    public Dictionary<string, object?> Context { get; set; } = new();
}
