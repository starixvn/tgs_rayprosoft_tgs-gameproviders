using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.Saba.Models
{
    public record SabaCreateMemberRequest
    {
        [JsonPropertyName("vendor_id")]
        public string VendorId { get; init; }

        [JsonPropertyName("vendor_member_id")]
        public string PlayerId { get; init; }

        [JsonPropertyName("username")]
        public string Username { get; init; }

        [JsonPropertyName("oddstype")]
        public string OddsType { get; init; }

        [JsonPropertyName("currency")]
        public string CurrencyCode { get; init; }

        [JsonPropertyName("operatorId")]
        public string OperatorId { get; init; }

        [JsonPropertyName("maxtransfer")]
        public decimal MaxTransfer => 1;

        [JsonPropertyName("mintransfer")]
        public decimal MinTransfer => 1;
    }
}
