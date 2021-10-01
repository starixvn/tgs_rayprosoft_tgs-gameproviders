using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.WGSports.Models.Commons;

namespace TGS.GameProviders.ApiClients.WGSports.Models
{
    public record WGSportsLoginResponse : WGSportsResponse
    {
        [JsonPropertyName("LoginUrl")]
        public string LoginUrl { get; set; }
        [JsonPropertyName("MemberId")]
        public string MemberId { get; set; }
    }
}
