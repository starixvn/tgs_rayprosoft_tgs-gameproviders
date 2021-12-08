using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.LY.Models.Commons
{
    public record LYBaseDataResponse
    {
        [JsonPropertyName("code")]
        public int Code { get; init; }
    }
}
