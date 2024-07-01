using System.Text.Json.Serialization;

namespace Clouded.Function.Library.Dtos;

public class ExecutionResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("logs")]
    public string? Logs { get; init; }

    [JsonPropertyName("data")]
    public string? Data { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; init; }
}
