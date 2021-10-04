using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGamHistoryRound.GameResult
{
    public record SlotGames
    {
        [JsonPropertyName("link")]
        public string Path { get; set; }
    }
}
