using System.Text.Json.Serialization;

namespace TGS.GameProviders.Domains.RoundDetail
{
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
