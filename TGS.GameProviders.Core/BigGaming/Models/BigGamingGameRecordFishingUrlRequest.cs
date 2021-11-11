using System.Text.Json.Serialization;
using TGS.GameProviders.Core.BigGaming.Models.Commons;

namespace TGS.GameProviders.Core.BigGaming.Models
{
    public record BigGamingGameRecordFishingUrlRequest : BigGamingParamsSignRequest
    {
        /// <summary>
        /// The external round identifier
        /// </summary>
        [JsonPropertyName("orderId")]
        public long ExternalBetId { get; set; }
        /// <summary>
        /// The game identifier
        /// </summary>
        [JsonPropertyName("gameType")]
        public int GameId { get; set; }
        /// <summary>
        /// The locale
        /// </summary>
        [JsonPropertyName("locale")]
        public string Locale;
    }
}
