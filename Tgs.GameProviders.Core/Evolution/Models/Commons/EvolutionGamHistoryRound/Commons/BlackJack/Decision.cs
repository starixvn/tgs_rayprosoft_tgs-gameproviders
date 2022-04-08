using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.BlackJack
{
    public record Decision
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
