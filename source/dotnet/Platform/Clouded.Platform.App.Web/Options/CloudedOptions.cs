using System.ComponentModel.DataAnnotations;

namespace Clouded.Platform.App.Web.Options;

public class CloudedOptions
{
    [Required]
    public string Domain { get; set; } = null!;

    [Required]
    public PlatformOptions Platform { get; set; } = null!;

    [Required]
    public AuthOptions Auth { get; set; } = null!;

    [Required]
    public DatabaseOptions Database { get; set; } = null!;

    [Required]
    public HubOptions Hub { get; set; } = null!;

    [Required]
    public ProviderOptions Provider { get; set; } = null!;

    [Required]
    public FunctionOptions Function { get; set; } = null!;

    [Required]
    public HarborOptions Harbor { get; set; } = null!;
}
