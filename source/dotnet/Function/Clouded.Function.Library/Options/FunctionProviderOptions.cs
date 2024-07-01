using System.ComponentModel.DataAnnotations;

namespace Clouded.Function.Library.Options;

public class FunctionProviderOptions
{
    [Required]
    public long Id { get; set; }

    [Required]
    public string Url { get; set; } = null!;

    [Required]
    public string ApiKey { get; set; } = null!;
}
