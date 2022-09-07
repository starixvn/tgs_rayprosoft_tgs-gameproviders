using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.GameResult;

public record FanTan
{
    [JsonPropertyName("buttonsCount")]
    public int ButtonsCount { get; set; }
}
