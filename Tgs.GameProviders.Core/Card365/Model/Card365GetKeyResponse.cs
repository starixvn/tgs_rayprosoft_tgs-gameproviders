using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Card365.Model;

public record Card365GetKeyResponse
{
    [JsonPropertyName("status")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int Status { get; init; }

    [JsonPropertyName("key")]
    public string Key { get; init; }
}
