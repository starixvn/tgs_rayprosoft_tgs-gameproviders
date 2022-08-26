using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.AsiaGaming.Models;

public record AsiaGamingGameUrlRequest : AsiaGamingBaseRequest
{
    [JsonPropertyName("dm")]
    public string ReturnUrl { get; init; } = default!;

    [JsonPropertyName("sid")]
    public string Sid { get; init; } = default!;

    [JsonPropertyName("mh5")]
    public string Mh5 { get; init; } = default!;

    [JsonPropertyName("lang")]
    public string Lang { get; init; } = default!;

    [JsonPropertyName("gameType")]
    public string GameId { get; init; } = default!;
}
