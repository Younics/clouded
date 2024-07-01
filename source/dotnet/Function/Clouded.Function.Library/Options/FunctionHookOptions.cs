using System.ComponentModel.DataAnnotations;

namespace Clouded.Function.Library.Options;

public class FunctionHookOptions
{
    [Required]
    public FunctionHookMethodOptions[] Validation { get; set; } =
        Array.Empty<FunctionHookMethodOptions>();

    [Required]
    public FunctionHookMethodOptions[] TransformInput { get; set; } =
        Array.Empty<FunctionHookMethodOptions>();

    [Required]
    public FunctionHookMethodOptions[] TransformOutput { get; set; } =
        Array.Empty<FunctionHookMethodOptions>();

    [Required]
    public FunctionHookMethodOptions[] HookBefore { get; set; } =
        Array.Empty<FunctionHookMethodOptions>();

    [Required]
    public FunctionHookMethodOptions[] HookAfter { get; set; } =
        Array.Empty<FunctionHookMethodOptions>();
}
