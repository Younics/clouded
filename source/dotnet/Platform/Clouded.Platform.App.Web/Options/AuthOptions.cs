using System.ComponentModel.DataAnnotations;

namespace Clouded.Platform.App.Web.Options;

public class AuthOptions
{
    [Required]
    public string ServerUrl { get; set; } = null!;

    [Required]
    public string ApiKey { get; set; } = null!;

    [Required]
    public string SecretKey { get; set; } = null!;

    [Required]
    public TokenOptions Token { get; set; } = null!;
}
