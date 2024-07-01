using System.ComponentModel.DataAnnotations;

namespace Clouded.Function.Api.Options;

public class ApplicationOptions
{
    [Required]
    public CloudedOptions Clouded { get; set; }
}
