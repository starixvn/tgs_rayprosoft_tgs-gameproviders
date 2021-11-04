using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.KY.Models.Commons
{
    public record KYApiRequest
    {
        /// <summary>
        /// The agent
        /// </summary>
        [JsonPropertyName("agent")]
        public string Agent { get; init; }

        /// <summary>
        /// The timestamp
        /// </summary>
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; init; }

        /// <summary>
        /// The parameter
        /// </summary>
        [JsonPropertyName("param")]
        public string Param { get; init; }

        /// <summary>
        /// The key
        /// </summary>
        [JsonPropertyName("key")]
        public string Key { get; set; }
    }
}
