using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.SexyGaming.Models.Commons
{
    public record SexyGamingBaseResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("desc")]
        public string Desc { get; set; }
    }
}
