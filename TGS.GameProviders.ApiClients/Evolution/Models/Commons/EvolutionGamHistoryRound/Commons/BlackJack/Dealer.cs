using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.BlackJack
{
    public record Dealer
    {
        [JsonPropertyName("score")]
        public int Score { get; set; }
        [JsonPropertyName("cards")]
        public string[] Cards { get; set; }
        [JsonPropertyName("bonusCards")]
        public object[] BonusCards { get; set; }
    }
}
