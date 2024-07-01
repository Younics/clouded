using System.ComponentModel.DataAnnotations;

namespace Clouded.Auth.Provider.Options;

public class ApplicationOptions
{
    [Required]
    public CloudedOptions Clouded { get; set; } = null!;
}
