using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.Skywind.Models
{
    public class SkywindGameUrlRequest
    {
        [JsonPropertyName("gameCode")]
        public string GameCode { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; } = "zh-cn";

        [JsonPropertyName("playerCode")]
        public string PlayerCode { get; set; }

        [JsonPropertyName("ticket")]
        public string Ticket { get; set; }
    }
}
