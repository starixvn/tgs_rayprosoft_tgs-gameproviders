using System.Text.Json.Serialization;
using Tgs.GameProviders.ApiClients.KY.Models.Commons;

namespace Tgs.GameProviders.ApiClients.KY.Models
{
    public record KYGameUrlResponse : KYBaseDataResponse
    {
        [JsonPropertyName("url")]
        public string Url { get; init; } = default!;
    }
}
