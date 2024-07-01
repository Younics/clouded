using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Clouded.Function.Framework.Contexts.Base;
using Clouded.Function.Shared;

namespace Clouded.Function.Library.Dtos;

public class HookRequest
{
    [Required]
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;

    [Required]
    [JsonPropertyName("type")]
    public EFunctionType Type { get; init; }

    [Required]
    [JsonPropertyName("context")]
    public HookContext? Context { get; init; }
}
