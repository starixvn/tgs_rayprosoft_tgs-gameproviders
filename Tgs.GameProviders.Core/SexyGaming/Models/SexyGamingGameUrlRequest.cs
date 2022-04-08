using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.SexyGaming.Models.Commons;

namespace Tgs.GameProviders.Core.SexyGaming.Models
{
    public record SexyGamingGameUrlRequest : SexyGamingBaseRequest
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }
        [JsonPropertyName("isMobileLogin")]
        public bool IsMobileLogin { get; set; }
        [JsonPropertyName("platform")]
        public string Platform => "SEXYBCRT";
        [JsonPropertyName("gameType")]
        public string GameType => "LIVE";
        [JsonPropertyName("language")]
        public string Language { get; set; } = "en";
        [JsonPropertyName("extension1")]
        public string Extension1 { get; set; }
    }
}
