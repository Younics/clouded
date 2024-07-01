using System.ComponentModel.DataAnnotations;

namespace Clouded.Platform.Hub.Options;

public class ApplicationOptions
{
    [Required]
    public CloudedOptions Clouded { get; set; }
}
