// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.IMESports.Models
{
    public record IMESportsGetRoundHistoryRequest
    {
        [JsonPropertyName("MerchantCode")]
        public string MerchantCode { get; init; }

        [JsonPropertyName("StartDate")]
        public string StartDate { get; init; }

        [JsonPropertyName("EndDate")]
        public string EndDate { get; init; }

        [JsonPropertyName("Page")]
        public int Page { get; init; }

        [JsonPropertyName("ProductWallet")]
        public int ProductWallet { get; init; }

        [JsonPropertyName("DateFilterType")]
        public int DateFilterType => 1;

        [JsonPropertyName("Language")]
        public string Language { get; init; }
    }
}