using System;
using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound;

namespace Tgs.GameProviders.Core.Evolution.Models
{
    public record EvolutionGamHistoryRoundResponse
    {
        [JsonPropertyName("data")]
        public EvolutionGamHistoryRoundData Data { get; set; }
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }
    }
}
