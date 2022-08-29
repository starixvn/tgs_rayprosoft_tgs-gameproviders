using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons;

namespace Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.GameResult;

public record Baccarat
{
    [JsonPropertyName("banker")]
    public Card Banker { get; set; }
    [JsonPropertyName("outcome")]
    public string Outcome { get; set; }
    [JsonPropertyName("player")]
    public Card Player { get; set; }
}
