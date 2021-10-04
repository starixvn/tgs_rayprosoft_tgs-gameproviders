using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.GameProviders.ApiClients.Evolution.Models
{
    public record EvolutionGamHistoryRoundRequest
    {
        public string PlayerCode { get; set; }
        public string RoundId { get; set; }
    }
}
