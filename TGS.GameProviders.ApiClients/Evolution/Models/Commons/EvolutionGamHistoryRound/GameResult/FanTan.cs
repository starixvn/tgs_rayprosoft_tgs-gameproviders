using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGamHistoryRound.GameResult
{
    public record FanTan
    {
        [JsonPropertyName("buttonsCount")]
        public int ButtonsCount { get; set; }
    }
}
