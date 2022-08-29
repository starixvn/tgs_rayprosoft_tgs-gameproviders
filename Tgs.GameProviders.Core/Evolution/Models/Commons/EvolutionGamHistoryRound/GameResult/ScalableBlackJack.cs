using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.ScalableBlackJack;

namespace Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.GameResult;

public record ScalableBlackJack
{
    [JsonPropertyName("dealtToPlayer")]
    public string[] DealtToPlayer { get; set; }
    [JsonPropertyName("dealerHand")]
    public Dealerhand DealerHand { get; set; }
    [JsonPropertyName("wonSideBets")]
    public object[] WonSideBets { get; set; }
}
