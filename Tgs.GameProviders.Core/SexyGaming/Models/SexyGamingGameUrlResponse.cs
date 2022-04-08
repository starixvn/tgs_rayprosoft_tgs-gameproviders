using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.SexyGaming.Models.Commons;

namespace Tgs.GameProviders.Core.SexyGaming.Models
{
    public record SexyGamingGameUrlResponse : SexyGamingBaseResponse
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
