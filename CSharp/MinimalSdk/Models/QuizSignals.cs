using System.Text.Json;
using System.Text.Json.Serialization;

namespace MinimalSdk.Models;

public record QuizSignals
{
    [JsonPropertyName("response")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Response { get; init; } = "";

    [JsonPropertyName("answer")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Answer { get; init; } = "";

    public string Serialize() => JsonSerializer.Serialize(this);
}
