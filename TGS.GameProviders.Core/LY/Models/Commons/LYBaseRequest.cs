using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.LY.Models.Commons
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
