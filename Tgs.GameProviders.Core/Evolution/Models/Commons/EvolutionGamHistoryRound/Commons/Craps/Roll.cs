using System;
using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.Craps
{
    public record Roll
    {
        [JsonPropertyName("rollId")]
        public string RollId { get; set; }
        [JsonPropertyName("startedAt")]
        public DateTimeOffset StartedAt { get; set; }
        [JsonPropertyName("result")]
        public Result Result { get; set; }
    }
}
