using System.Text.Json.Serialization;
using TGS.GameProviders.Core.KY.Models.Commons;

namespace TGS.GameProviders.Core.KY.Models
{
    public record KYGameUrlResponse : KYBaseDataResponse
    {
        [JsonPropertyName("url")]
        public string Url { get; init; }
    }
}
