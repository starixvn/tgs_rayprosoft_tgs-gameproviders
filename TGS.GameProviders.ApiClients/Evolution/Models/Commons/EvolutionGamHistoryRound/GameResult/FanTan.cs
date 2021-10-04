using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGamHistoryRound.GameResult
{
    public record FanTan
    {
        [JsonPropertyName("buttonsCount")]
        public int ButtonsCount { get; set; }
    }
}
