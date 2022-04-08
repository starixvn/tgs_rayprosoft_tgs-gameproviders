using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.BigGaming.Models.Commons;

namespace Tgs.GameProviders.Core.BigGaming.Models
{
    public record BigGamingGetTrialGameUrlRequest : BigGamingParamsSignRequest
    {
        [JsonPropertyName("locale")]
        public string Locale { get; init; } = default!;
        [JsonPropertyName("isMobileUrl")]
        public int IsMobileUrl { get; init; }
        [JsonPropertyName("isHttpsUrl")]
        public int IsHttpsUrl { get; init; }
        [JsonPropertyName("returnUrl")]
        public string ReturnUrl { get; init; } = default!;
    }
}
