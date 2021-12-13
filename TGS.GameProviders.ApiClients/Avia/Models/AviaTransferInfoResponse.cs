using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.Avia.Models
{
    internal record AviaTransferInfoResponse
    {
        [JsonPropertyName("Error")]
        public string Error { get; init; }

        [JsonPropertyName("ID")]
        public string ID { get; set; }
        [JsonPropertyName("Currency")]
        public string Currency { get; set; }
        [JsonPropertyName("CreateAt")]
        public string CreateAt { get; set; }
        [JsonPropertyName("Money")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal Money { get; set; }

        [JsonPropertyName("UserName")]
        public string UserName { get; set; }
        [JsonPropertyName("Type")]
        public string Type { get; set; }

        [JsonPropertyName("Status")]
        public string Status { get; set; }

        [JsonPropertyName("Balance")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal Balance { get; set; }

        [JsonPropertyName("BalanceBefore")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal BalanceBefore { get; set; }
    }
}
