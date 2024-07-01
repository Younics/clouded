using System.ComponentModel.DataAnnotations;

namespace Clouded.Function.Api.Options;

public class FunctionOptions
{
    [Required]
    public string ApiKey { get; set; } = null!;

    [Required]
    public Dictionary<string, string> Hooks { get; set; } = new();
}
