using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.ScalableBlackJack
{
    public record Dealerhand
    {
        [JsonPropertyName("score")]
        public int Score { get; set; }
        [JsonPropertyName("cards")]
        public string[] Cards { get; set; }
    }
}
