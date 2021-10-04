using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.GameProviders.ApiClients.Skywind.Models
{
    public class SkywindGameUrlRequest
    {
        [JsonPropertyName("playerCode")]
        public string PlayerCode { get; set; }
        [JsonPropertyName("gameCode")]
        public string GameCode { get; set; }
        [JsonPropertyName("ticket")]
        public string Ticket { get; set; }
        [JsonPropertyName("language")]
        public string Language { get; set; } = "zh-cn";
    }
}
