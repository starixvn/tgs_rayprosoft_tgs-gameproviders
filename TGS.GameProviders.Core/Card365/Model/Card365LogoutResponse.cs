using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.Card365.Model;

public record Card365LogoutResponse
{
    [JsonPropertyName("status")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int Status { get; init; }
}
