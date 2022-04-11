using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Skywind.Models
{
    public record SkywindGamHistoryRoundDetailsResponse
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; }
        [JsonPropertyName("ttl")]
        public int Ttl { get; set; }
    }
}
