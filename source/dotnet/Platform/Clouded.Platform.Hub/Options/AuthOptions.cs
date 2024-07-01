using System.ComponentModel.DataAnnotations;

namespace Clouded.Platform.Hub.Options;

public class AuthOptions
{
    [Required]
    public TokenOptions Token { get; set; }
}
