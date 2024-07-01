using System.ComponentModel.DataAnnotations;

namespace Clouded.Platform.Hub.Options;

public class ProviderOptions
{
    [Required]
    public string ApiKey { get; set; } = null!;

    [Required]
    public Dictionary<string, string> RegionServerUrls { get; set; } = null!;
}
