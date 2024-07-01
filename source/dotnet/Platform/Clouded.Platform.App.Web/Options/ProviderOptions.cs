using System.ComponentModel.DataAnnotations;

namespace Clouded.Platform.App.Web.Options;

public class ProviderOptions
{
    [Required]
    public string HostIpAddress { get; set; } = null!;

    [Required]
    public DockerOptions Docker { get; set; } = null!;
}
