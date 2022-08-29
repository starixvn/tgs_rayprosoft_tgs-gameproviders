using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.JDB.Models;

public record JDBLogoutRequest
{
    [JsonPropertyName("action")]
    public int Action => 17;

    [JsonPropertyName("ts")]
    public long Timestamp { get; init; }

    [JsonPropertyName("parent")]
    public string Parent { get; init; }

    [JsonPropertyName("uid")]
    public string UserId { get; init; }
}
