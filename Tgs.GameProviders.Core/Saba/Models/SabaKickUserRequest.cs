using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Saba.Models
{
    public record SabaKickUserRequest
    {
        [JsonPropertyName("vendor_id")]
        public string VendorId { get; init; }

        [JsonPropertyName("vendor_member_id")]
        public string PlayerId { get; init; }
    }
}
