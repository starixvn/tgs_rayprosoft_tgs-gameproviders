using System.Text.Json.Serialization;
using TGS.GameProviders.Core.WGSports.Models.Commons;

namespace TGS.GameProviders.Core.WGSports.Models
{
    public record WGSportsLoginResponse : WGSportsResponse
    {
        [JsonPropertyName("LoginUrl")]
        public string LoginUrl { get; set; }
        [JsonPropertyName("MemberId")]
        public string MemberId { get; set; }
    }
}
