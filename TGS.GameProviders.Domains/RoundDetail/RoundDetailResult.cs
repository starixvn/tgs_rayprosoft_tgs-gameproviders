using System.Text.Json.Serialization;

namespace TGS.GameProviders.Domains.RoundDetail
{
    public record RoundDetailResult
    {
        [JsonPropertyName("data")]
        public RoundDetailDataBase[]? Data { get; init; }
    }
}
