using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons
{
    public record DragonTigerCard
    {
        [JsonPropertyName("card")]
        public string Card { get; set; }
        [JsonPropertyName("score")]
        public int Score { get; set; }
    }
}
