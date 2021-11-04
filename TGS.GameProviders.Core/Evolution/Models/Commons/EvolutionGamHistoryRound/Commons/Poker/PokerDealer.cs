using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.Poker
{
    public record PokerDealer
    {
        [JsonPropertyName("cards")]
        public string[] Cards { get; set; }
        [JsonPropertyName("qualified")]
        public bool Qualified { get; set; }
        [JsonPropertyName("rank")]
        public int Rank { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
