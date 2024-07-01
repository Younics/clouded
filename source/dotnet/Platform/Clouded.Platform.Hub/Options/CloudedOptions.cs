using System.ComponentModel.DataAnnotations;
using Clouded.Platform.Models.Enums;

namespace Clouded.Platform.Hub.Options;

public class CloudedOptions
{
    [Required]
    public string Domain { get; set; } = null!;

    [Required]
    public ERegionCode RegionCode { get; set; }

    [Required]
    public DatabaseOptions Database { get; set; }

    [Required]
    public AuthOptions Auth { get; set; }

    [Required]
    public HubOptions Hub { get; set; }

    [Required]
    public ProviderOptions Provider { get; set; }

    [Required]
    public HarborOptions Harbor { get; set; }
}
