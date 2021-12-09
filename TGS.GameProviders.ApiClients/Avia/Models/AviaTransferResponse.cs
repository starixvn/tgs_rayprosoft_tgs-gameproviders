using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.Avia.Models
{
    internal record AviaTransferResponse
    {
        [JsonPropertyName("ID")]
        public int ID { get; init; }

        [JsonPropertyName("OrderID")]
        public string OrderID { get; init; }

        [JsonPropertyName("Balance")]
        public float Balance { get; init; }

        [JsonPropertyName("Credit")]
        public float Credit { get; init; }

        [JsonPropertyName("Currency")]
        public string Currency { get; init; }
    }
}
