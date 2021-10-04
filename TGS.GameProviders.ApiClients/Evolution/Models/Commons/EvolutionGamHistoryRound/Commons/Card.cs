using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons
{
    public record Card
    {
        [JsonPropertyName("cards")]
        public string[] Cards { get; set; }
        [JsonPropertyName("score")]
        public int Score { get; set; }
    }
}
