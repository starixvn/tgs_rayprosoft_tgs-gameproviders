using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.LY.Models.Commons;

namespace TGS.GameProviders.ApiClients.LY.Models
{
    public record LYGameUrlResponse : LYBaseDataResponse
    {
        [JsonPropertyName("url")]
        public string Url { get; init; }
    }
}
