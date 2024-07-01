using System.ComponentModel.DataAnnotations;

namespace Clouded.Platform.Provider.Options;

public class NugetOptions
{
    [Required]
    public string User { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}
