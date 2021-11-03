using System.Text.Json.Serialization;
using TGS.GameProviders.Core.WGHorse.Models.Commons;

namespace TGS.GameProviders.Core.WGHorse.Models
{
    public record WGHorseLoginResponse : WGHorseResponse
    {
        [JsonPropertyName("LoginUrl")]
        public string LoginUrl { get; set; }
        [JsonPropertyName("MemberId")]
        public string MemberId { get; set; }
    }
}
