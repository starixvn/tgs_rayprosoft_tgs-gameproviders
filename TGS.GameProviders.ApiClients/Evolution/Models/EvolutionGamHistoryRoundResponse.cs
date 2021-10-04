using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGamHistoryRound;

namespace TGS.GameProviders.ApiClients.Evolution.Models
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
