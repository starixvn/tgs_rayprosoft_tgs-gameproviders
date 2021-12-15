using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.LY.Models.Commons
{
    public record LYBaseRequest
    {
        /// <summary>
        /// The action
        /// </summary>
        [JsonPropertyName("s")]
        public int Action { get; set; }
    }
}
