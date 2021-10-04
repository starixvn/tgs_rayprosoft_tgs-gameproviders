using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.Poker
{
    public record PokerCards
    {
        [JsonPropertyName("dealer")]
        public string[] Dealer { get; set; }
        [JsonPropertyName("flop")]
        public string[] Flop { get; set; }
        [JsonPropertyName("player")]
        public string[] Player { get; set; }
        [JsonPropertyName("river")]
        public string[] River { get; set; }
    }
}
