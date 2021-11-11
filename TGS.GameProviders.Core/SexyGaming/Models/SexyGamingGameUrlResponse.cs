using System.Text.Json.Serialization;
using TGS.GameProviders.Core.SexyGaming.Models.Commons;

namespace TGS.GameProviders.Core.SexyGaming.Models
{
    public record SexyGamingGameUrlResponse : SexyGamingBaseResponse
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
