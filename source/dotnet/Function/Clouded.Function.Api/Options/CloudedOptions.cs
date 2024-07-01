using System.ComponentModel.DataAnnotations;

namespace Clouded.Function.Api.Options;

public class CloudedOptions
{
    [Required]
    public FunctionOptions Function { get; set; }
}
