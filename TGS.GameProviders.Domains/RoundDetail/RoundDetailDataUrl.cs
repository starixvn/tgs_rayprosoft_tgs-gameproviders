using System.Text.Json.Serialization;

namespace TGS.GameProviders.Domains.RoundDetail
{
    public record RoundDetailDataUrl : RoundDetailDataBase
    {
        public RoundDetailDataUrl() : base(RoundDetailDataType.Url)
        {
        }

        [JsonPropertyName("url")]
        public string Url { get; init; } = default!;
    }
}
