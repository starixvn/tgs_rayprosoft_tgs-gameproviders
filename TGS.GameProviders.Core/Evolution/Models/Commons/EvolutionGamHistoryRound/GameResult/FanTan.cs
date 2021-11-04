using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.GameResult
{
    public record FanTan
    {
        [JsonPropertyName("buttonsCount")]
        public int ButtonsCount { get; set; }
    }
}
