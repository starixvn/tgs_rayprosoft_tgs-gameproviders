using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.AsiaGaming.Models
{
    public record AsiaGamingGameUrlRequest : AsiaGamingBaseRequest
    {
        [JsonPropertyName("dm")]
        public string ReturnUrl { get; init; }

        [JsonPropertyName("sid")]
        public string Sid { get; init; }

        [JsonPropertyName("mh5")]
        public string Mh5 { get; init; }

        [JsonPropertyName("lang")]
        public string Lang { get; init; }

        [JsonPropertyName("gameType")]
        public string GameId { get; init; }
    }
}