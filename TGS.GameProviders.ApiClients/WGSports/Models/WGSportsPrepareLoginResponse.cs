using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.WGSports.Models.Commons;

namespace TGS.GameProviders.ApiClients.WGSports.Models
{
    public record WGSportsPrepareLoginResponse : WGSportsResponse
    {
        [JsonPropertyName("Token")]
        public string Token { get; init; } = default!;
    }
}
