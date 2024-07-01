using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Clouded.Function.Framework.Contexts;

namespace Clouded.Function.Library.Dtos;

public class FunctionRequest
{
    [Required]
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [Required]
    [JsonPropertyName("context")]
    public FunctionContext Context { get; set; }
}
