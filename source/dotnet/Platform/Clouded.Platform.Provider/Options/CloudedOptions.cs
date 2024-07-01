using System.ComponentModel.DataAnnotations;

namespace Clouded.Platform.Provider.Options;

public class CloudedOptions
{
    [Required]
    public string Domain { get; set; } = null!;

    [Required]
    public long RegionId { get; set; }

    [Required]
    public AuthOptions Auth { get; set; } = null!;

    [Required]
    public HubOptions Hub { get; set; } = null!;

    [Required]
    public ProviderOptions Provider { get; set; } = null!;

    [Required]
    public HarborOptions Harbor { get; set; } = null!;

    [Required]
    public NugetOptions Nuget { get; set; } = null!;
}
