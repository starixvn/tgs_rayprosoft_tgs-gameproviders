using System.Text.Json.Serialization;

namespace TGS.GameProviders.Domains.RoundDetail
{
    [JsonConverter(typeof(RoundDetailDataConverter))]
    public abstract record RoundDetailDataBase
    {
        protected RoundDetailDataBase(RoundDetailDataType type)
        {
            Type = type;
        }

        [JsonPropertyName("type")]
        public RoundDetailDataType Type { get; }
    }
}
