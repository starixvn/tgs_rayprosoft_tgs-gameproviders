using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.Poker
{
    public record PokerBonusTripsbonus
    {
        [JsonPropertyName("cards")]
        public string[] Cards { get; set; }
        [JsonPropertyName("handType")]
        public string HandType { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
