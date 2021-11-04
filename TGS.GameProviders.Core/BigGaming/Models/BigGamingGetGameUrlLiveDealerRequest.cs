using System.Text.Json.Serialization;
using TGS.GameProviders.Core.BigGaming.Models.Commons;

namespace TGS.GameProviders.Core.BigGaming.Models
{
    public record BigGamingGetGameUrlLiveDealerRequest : BigGamingParamsDigestRequest
    {
        [JsonPropertyName("loginId")]
        public string LoginId { get; init; }
        [JsonPropertyName("locale")]
        public string Locale { get; init; }
        [JsonPropertyName("isMobileUrl")]
        public int IsMobileUrl { get; init; }
        [JsonPropertyName("isHttpsUrl")]
        public int IsHttpsUrl { get; init; }
        [JsonPropertyName("returnUrl")]
        public string ReturnUrl { get; init; }
        [JsonPropertyName("fromIp")]
        public string FromIp { get; init; }
    }
}
