using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.BlackJack
{
    public record Decision
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
