using System.Text.Json.Serialization;
using TGS.GameProviders.Core.VR.Models.Commons;

namespace TGS.GameProviders.Core.VR.Models
{
    public record VRGetBalanceResponse
    {
        [JsonPropertyName("playerName")]
        public string PlayerName { get; init; }
        [JsonPropertyName("balance")]
        public decimal Balance { get; init; }
        [JsonPropertyName("games")]
        public VRGetBalanceGamesResponse[] Games { get; init; }
    }
}
