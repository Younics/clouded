using System.ComponentModel.DataAnnotations;

namespace Clouded.Function.Library.Options;

public class FunctionOptions
{
    [Required]
    public IEnumerable<FunctionProviderOptions> Providers { get; set; } = null!;

    [Required]
    public Dictionary<string, FunctionHookOptions> Hooks { get; set; } = null!;
}
