using System.Text.Json.Serialization;
using TGS.GameProviders.Core.WGHorse.Models.Commons;

namespace TGS.GameProviders.Core.WGHorse.Models
{
    public record WGHorseGetBalanceResponse : WGHorseResponse
    {
        [JsonPropertyName("Balance")]
        public string Balance { get; init; }
        [JsonPropertyName("Currency")]
        public string Currency { get; init; }
    }
}
