using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.BigGaming.Models.Commons;

namespace Tgs.GameProviders.Core.BigGaming.Models
{
    public record BigGamingGameRecordFishingUrlRequest : BigGamingParamsSignRequest
    {
        /// <summary>
        /// The external round identifier
        /// </summary>
        [JsonPropertyName("orderId")]
        public long ExternalBetId { get; init; }
        /// <summary>
        /// The game identifier
        /// </summary>
        [JsonPropertyName("gameType")]
        public int GameId { get; init; }
        /// <summary>
        /// The locale
        /// </summary>
        [JsonPropertyName("locale")]
        public string Locale { get; init; } = default!;
    }
}
