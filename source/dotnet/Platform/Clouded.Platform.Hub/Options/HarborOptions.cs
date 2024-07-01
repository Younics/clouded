using System.ComponentModel.DataAnnotations;

namespace Clouded.Platform.Hub.Options;

public class HarborOptions
{
    [Required]
    public string Server { get; set; } = null!;
}
