using System.ComponentModel.DataAnnotations;

namespace Clouded.Platform.Provider.Options;

public class HarborOptions
{
    [Required]
    public string Server { get; set; } = null!;

    [Required]
    public string User { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}
