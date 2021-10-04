using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGameUrl
{
    public record EvolutionGameUrlConfigUrls
    {
        [JsonPropertyName("cashier")]
        public string Cashier { get; set; }
        [JsonPropertyName("responsibleGaming")]
        public string ResponsibleGaming { get; set; }
        [JsonPropertyName("lobby")]
        public string Lobby { get; set; }
        [JsonPropertyName("sessionTimeout")]
        public string SessionTimeout { get; set; }
    }
}
