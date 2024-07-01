using System.ComponentModel.DataAnnotations;

namespace Clouded.Platform.Provider.Options;

public class HubOptions
{
    [Required]
    public string ApiKey { get; set; } = null!;

    [Required]
    public Dictionary<string, string> RegionServerUrls { get; set; } = null!;
}
