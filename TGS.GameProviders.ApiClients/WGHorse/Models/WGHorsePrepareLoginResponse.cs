using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.WGHorse.Models.Commons;

namespace TGS.GameProviders.ApiClients.WGHorse.Models
{
    public record WGHorsePrepareLoginResponse : WGHorseResponse
    {
        [JsonPropertyName("Token")]
        public string Token { get; init; }
    }
}
