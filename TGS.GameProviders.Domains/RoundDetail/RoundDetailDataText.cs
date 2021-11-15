using System.Text.Json.Serialization;

namespace TGS.GameProviders.Domains.RoundDetail
{
    public record RoundDetailDataText : RoundDetailDataBase
    {
        public RoundDetailDataText() : base(RoundDetailDataType.Text)
        {
        }

        [JsonPropertyName("value")]
        public string Value { get; init; } = default!;
    }
}
