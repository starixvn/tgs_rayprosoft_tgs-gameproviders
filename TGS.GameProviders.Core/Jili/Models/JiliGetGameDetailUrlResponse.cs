using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.Jili.Models;

public record JiliGetGameDetailUrlResponse
{
    [JsonPropertyName("ErrorCode")]
    public int ErrorCode { get; init; }

    [JsonPropertyName("Message")]
    public string ErrorMessage { get; init; }

    [JsonPropertyName("Data")]
    public JiliGetGameDetailUrlData Data { get; init; }
}