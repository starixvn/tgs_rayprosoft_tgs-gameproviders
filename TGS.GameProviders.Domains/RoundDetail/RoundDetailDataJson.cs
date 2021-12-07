using System.Text.Json.Serialization;

namespace TGS.GameProviders.Domains.RoundDetail
{
    public record RoundDetailDataJson : RoundDetailDataBase
    {
        public RoundDetailDataJson() : base(RoundDetailDataType.Json)
        {
        }

        [JsonPropertyName("value")]
        public string? Value { get; init; } = default!;
    }
}