using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.SideBetCity;

namespace Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.GameResult;

public record SideBetCity
{
    [JsonPropertyName("cardsDealt")]
    public string[] CardsDealt { get; set; }
    [JsonPropertyName("outcome")]
    public SideBetCityOutcome Outcome { get; set; }
}
