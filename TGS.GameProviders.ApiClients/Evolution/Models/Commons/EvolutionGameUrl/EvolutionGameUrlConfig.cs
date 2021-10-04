using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGameUrl
{
    public record EvolutionGameUrlConfig
    {
        [JsonPropertyName("brand")]
        public EvolutionGameUrlConfigBrand Brand { get; set; }
        [JsonPropertyName("game")]
        public EvolutionGameUrlConfigGame Game { get; set; }
        [JsonPropertyName("channel")]
        public EvolutionGameUrlConfigChannel Channel { get; set; }
        [JsonPropertyName("urls")]
        public EvolutionGameUrlConfigUrls Urls { get; set; }
        [JsonPropertyName("freeGames")]
        public bool FreeGames { get; set; }
    }
}
