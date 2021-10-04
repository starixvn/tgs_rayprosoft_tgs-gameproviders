using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.SexyGaming.Models.Commons;

namespace TGS.GameProviders.ApiClients.SexyGaming.Models
{
    public record SexyGamingGameUrlResponse : SexyGamingBaseResponse
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
