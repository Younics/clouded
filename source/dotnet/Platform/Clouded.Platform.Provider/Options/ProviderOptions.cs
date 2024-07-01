using System.ComponentModel.DataAnnotations;

namespace Clouded.Platform.Provider.Options;

public class ProviderOptions
{
    [Required]
    public string ApiKey { get; set; } = null!;

    [Required]
    public ProviderDockerOptions Docker { get; set; } = null!;

    [Required]
    public ProviderKubernetesOptions Kubernetes { get; set; } = null!;
}
