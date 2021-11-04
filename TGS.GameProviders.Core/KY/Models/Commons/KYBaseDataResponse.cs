using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.KY.Models.Commons
{
    public record KYBaseDataResponse
    {
        [JsonPropertyName("code")]
        public int Code { get; init; }
    }
}
