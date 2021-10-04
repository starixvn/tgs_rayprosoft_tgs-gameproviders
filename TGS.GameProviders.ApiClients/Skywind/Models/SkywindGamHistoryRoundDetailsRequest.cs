using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.Skywind.Models
{
    public record SkywindGamHistoryRoundDetailsRequest
    {
        [JsonPropertyName("roundId")]
        public string RoundId { get; set; }
        [JsonPropertyName("language")]
        public string Language { get; set; } = "zh-cn";
    }
}
