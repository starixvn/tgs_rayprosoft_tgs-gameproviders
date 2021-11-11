using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.KY.Models.Commons
{
    public record KYApiResponse<TData>
    {
        [JsonPropertyName("m")]
        public string Message { get; init; }

        [JsonPropertyName("s")]
        public int SubMethod { get; init; }

        [JsonPropertyName("d")]
        public TData Data { get; init; }
    }
}
