using System.Text.Json.Serialization;

namespace Mvc.Models;

public record ClickToLoadSignals
{
    [JsonPropertyName("offset")]
    public int Offset { get; init; }
    
    [JsonPropertyName("limit")]
    public int Limit { get; init; }
}
