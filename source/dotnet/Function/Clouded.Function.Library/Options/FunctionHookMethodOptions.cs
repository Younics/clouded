using System.ComponentModel.DataAnnotations;

namespace Clouded.Function.Library.Options;

public class FunctionHookMethodOptions
{
    [Required]
    public long ProviderId { get; set; }

    [Required]
    public int Index { get; set; }

    [Required]
    public string Name { get; set; } = null!;
}
