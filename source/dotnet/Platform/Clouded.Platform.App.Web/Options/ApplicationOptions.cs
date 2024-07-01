using System.ComponentModel.DataAnnotations;
using Clouded.Core.Tracking.Options;

namespace Clouded.Platform.App.Web.Options;

public class ApplicationOptions
{
    [Required]
    public CloudedOptions Clouded { get; set; } = null!;

    [Required]
    public TrackingOptions Tracking { get; set; } = null!;

    [Required]
    public WebSupportOptions WebSupport { get; set; } = null!;

    [Required]
    public HarborOptions Harbor { get; set; } = null!;

    [Required]
    public OpenAIOptions OpenAi { get; set; } = null!;
}
