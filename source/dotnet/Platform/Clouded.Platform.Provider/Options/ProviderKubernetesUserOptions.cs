using System.ComponentModel.DataAnnotations;

namespace Clouded.Platform.Provider.Options;

public class ProviderKubernetesUserOptions
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string ClientCertificateData { get; set; } = null!;

    [Required]
    public string ClientKeyData { get; set; } = null!;
}
