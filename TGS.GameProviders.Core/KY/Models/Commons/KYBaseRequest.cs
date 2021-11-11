using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.KY.Models.Commons
{
    public record KYBaseRequest
    {
        /// <summary>
        /// The action
        /// </summary>
        [JsonPropertyName("s")]
        public int Action { get; set; }
    }
}
