using System;
using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.VR.Models.Commons
{
    public record VRFundCheckingRecordResponse
    {
        [JsonPropertyName("serialNumber")]
        public string SerialNumber { get; init; }
        [JsonPropertyName("playerName")]
        public string PlayerCode { get; init; }
        [JsonPropertyName("type")]
        public int Type { get; init; }
        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }
        public DateTime createTime { get; init; }
        [JsonPropertyName("state")]
        public int state { get; init; }
        [JsonPropertyName("balance")]
        public decimal balance { get; init; }
    }
}
