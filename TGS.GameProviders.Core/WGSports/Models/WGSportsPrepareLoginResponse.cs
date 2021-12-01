using System.Text.Json.Serialization;
using TGS.GameProviders.Core.WGSports.Models.Commons;

namespace TGS.GameProviders.Core.WGSports.Models
{
    public record WGSportsPrepareLoginResponse : WGSportsResponse
    {
        [JsonPropertyName("Token")]
        public string Token { get; init; } = default!;
    }
}
