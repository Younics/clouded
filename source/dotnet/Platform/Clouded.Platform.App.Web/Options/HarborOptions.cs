using System.ComponentModel.DataAnnotations;

namespace Clouded.Platform.App.Web.Options;

public class HarborOptions
{
    [Required]
    public string ServerUrl { get; set; } = null!;

    [Required]
    public string User { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}
