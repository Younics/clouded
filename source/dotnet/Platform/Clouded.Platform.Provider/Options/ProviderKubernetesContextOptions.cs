using System.ComponentModel.DataAnnotations;

namespace Clouded.Platform.Provider.Options;

public class ProviderKubernetesContextOptions
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Cluster { get; set; } = null!;

    [Required]
    public string User { get; set; } = null!;
}
