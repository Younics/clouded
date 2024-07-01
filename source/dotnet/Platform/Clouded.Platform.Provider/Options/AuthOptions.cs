using System.ComponentModel.DataAnnotations;

namespace Clouded.Platform.Provider.Options;

public class AuthOptions
{
    [Required]
    public string ServerUrl { get; set; } = null!;

    [Required]
    public string ApiKey { get; set; } = null!;

    [Required]
    public TokenOptions Token { get; set; } = null!;
}
