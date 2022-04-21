using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.BigGaming.Models.Commons
{
    public record BigGamingParamsSignRequest
    {
        [JsonPropertyName("random")]
        public string Random { get; set; }
        [JsonPropertyName("sign")]
        public string Sign { get; set; }
        [JsonPropertyName("sn")]
        public string Sn { get; set; }
    }
}
