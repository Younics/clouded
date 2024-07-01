using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Clouded.Platform.Provider.Options;

public class ProviderKubernetesOptions
{
    [Required]
    public string Version { get; set; } = null!;

    [Required]
    public string RepositoryAuthBase64 { get; set; } = null!;

    public string RepositoryAuth =>
        Encoding.UTF8.GetString(Convert.FromBase64String(RepositoryAuthBase64));

    [Required]
    public string CurrentContext { get; set; } = null!;

    [Required]
    public Dictionary<string, object> Preferences { get; set; } = null!;

    [Required]
    public IEnumerable<ProviderKubernetesClusterOptions> Clusters { get; set; } = null!;

    [Required]
    public IEnumerable<ProviderKubernetesContextOptions> Contexts { get; set; } = null!;

    [Required]
    public IEnumerable<ProviderKubernetesUserOptions> Users { get; set; } = null!;
}
