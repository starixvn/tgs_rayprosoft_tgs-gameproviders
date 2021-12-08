using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.WGHorse.Models.Commons;

namespace TGS.GameProviders.ApiClients.WGHorse.Models
{
    public record WGHorseLoginResponse : WGHorseResponse
    {
        [JsonPropertyName("LoginUrl")]
        public string LoginUrl { get; set; }
        [JsonPropertyName("MemberId")]
        public string MemberId { get; set; }
    }
}
