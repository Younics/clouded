using System.ComponentModel.DataAnnotations;

namespace Clouded.Platform.Hub.Options;

public class HubOptions
{
    [Required]
    public string ApiKey { get; set; } = null!;
}
