using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.Avia.Models
{
    internal record AviaTransferInfoResponse
    {
        [JsonPropertyName("ID")]
        public string ID { get; set; }
        [JsonPropertyName("Currency")]
        public string Currency { get; set; }
        [JsonPropertyName("CreateAt")]
        public string CreateAt { get; set; }
        [JsonPropertyName("Money")]
        public string Money { get; set; }
        [JsonPropertyName("UserName")]
        public string UserName { get; set; }
        [JsonPropertyName("Type")]
        public string Type { get; set; }
        [JsonPropertyName("Status")]
        public AviaTransferInfoStatus Status { get; set; }
        [JsonPropertyName("Balance")]
        public string Balance { get; set; }
        [JsonPropertyName("BalanceBefore")]
        public string BalanceBefore { get; set; }
        [JsonPropertyName("CurrentBalance")]
        public string CurrentBalance { get; set; }
        [JsonPropertyName("Credit")]
        public string Credit { get; set; }
    }
}
