using System.Text.Json.Serialization;
using TGS.GameProviders.Core.WGHorse.Models.Commons;

namespace TGS.GameProviders.Core.WGHorse.Models
{
    public record WGHorsePrepareLoginResponse : WGHorseResponse
    {
        [JsonPropertyName("Token")]
        public string Token { get; init; }
    }
}
