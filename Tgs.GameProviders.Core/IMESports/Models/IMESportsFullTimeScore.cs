using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.IMESports.Models
{
    public class IMESportsFullTimeScore
    {
        [JsonPropertyName("Option")]
        public string Option { get; init; }

        [JsonPropertyName("OUScore")]
        public int OUScore { get; init; }

        [JsonPropertyName("Outcome")]
        public IMESportsOutcomeType Outcome { get; init; }
    }
}
