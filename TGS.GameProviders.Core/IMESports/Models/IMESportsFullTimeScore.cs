using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.IMESports.Models
{
    public class IMESportsFullTimeScore
    {
        [JsonPropertyName("OUScore")]
        public int OUScore { get; init; }

        [JsonPropertyName("Option")]
        public string Option { get; init; }

        [JsonPropertyName("Outcome")]
        public IMESportsOutcomeType Outcome { get; init; }
    }
}