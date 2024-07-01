using System.ComponentModel.DataAnnotations;

namespace Clouded.Platform.Provider.Options;

public class ProviderDockerOptions
{
    [Required]
    public string AuthProviderImage { get; set; } = null!;

    [Required]
    public string AdminProviderImage { get; set; } = null!;

    [Required]
    public string ApiProviderImage { get; set; } = null!;

    [Required]
    public string FunctionProviderImage { get; set; } = null!;
}
