using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Saba.Models
{
    public record SabaGetRoundHistoryRequest
    {
        [JsonPropertyName("vendor_id")]
        public string VendorId { get; init; }

        [JsonPropertyName("trans_id")]
        public string ExternalTransactionId { get; init; }
    }
}
