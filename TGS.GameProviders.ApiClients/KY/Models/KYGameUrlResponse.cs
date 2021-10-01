using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.KY.Models.Commons;

namespace TGS.GameProviders.ApiClients.KY.Models
{
    public record KYGameUrlResponse : KYBaseDataResponse
    {
        [JsonPropertyName("url")]
        public string Url { get; init; }
    }
}
