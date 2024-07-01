using System.ComponentModel.DataAnnotations;

namespace Clouded.Platform.Provider.Options;

public class ProviderKubernetesClusterOptions
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Server { get; set; } = null!;

    [Required]
    public string CertificateAuthorityData { get; set; } = null!;
}
