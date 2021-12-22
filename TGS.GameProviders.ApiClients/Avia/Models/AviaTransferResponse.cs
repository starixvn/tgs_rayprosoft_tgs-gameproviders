using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.Avia.Models
{
    internal record AviaTransferResponse
    {
        [JsonPropertyName("Error")]
        public string Error { get; init; }

        [JsonPropertyName("ID")]
        public string ID { get; init; }

        [JsonPropertyName("OrderID")]
        public string OrderID { get; init; }

        [JsonPropertyName("Balance")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal Balance { get; init; }

        [JsonPropertyName("Currency")]
        public string Currency { get; init; }
    }
}
