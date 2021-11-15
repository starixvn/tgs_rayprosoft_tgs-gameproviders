using System.Text.Json.Serialization;
using TGS.GameProviders.Core.BigGaming.Models.Commons;

namespace TGS.GameProviders.Core.BigGaming.Models
{
    public record BigGamingGetGameUrlFishingRequest : BigGamingParamsSignRequest
    {
        [JsonPropertyName("loginId")]
        public string LoginId { get; init; } = default!;
        [JsonPropertyName("locale")]
        public string Locale { get; init; } = default!;
        [JsonPropertyName("isMobileUrl")]
        public int IsMobileUrl { get; init; }
        [JsonPropertyName("isHttpsUrl")]
        public int IsHttpsUrl { get; init; }
        [JsonPropertyName("returnUrl")]
        public string ReturnUrl { get; init; } = default!;
        [JsonPropertyName("fromIp")]
        public string FromIp { get; init; } = default!;
        [JsonPropertyName("gameType")]
        public string GameType { get; set; } = default!;
    }
}
