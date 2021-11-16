using System.Text.Json.Serialization;

namespace TGS.GameProviders.Domains.RoundDetail
{
    public record RoundDetailDataXml : RoundDetailDataBase
    {
        public RoundDetailDataXml() : base(RoundDetailDataType.Xml)
        {
        }

        [JsonPropertyName("value")]
        public string Value { get; init; } = default!;
    }
}
