using System.ComponentModel.DataAnnotations;

namespace Clouded.Platform.Provider.Options;

public class ApplicationOptions
{
    [Required]
    public CloudedOptions Clouded { get; set; } = null!;
}
