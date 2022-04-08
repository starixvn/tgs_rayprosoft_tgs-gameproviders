using System;
using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.VR.Models
{
    public record VRFnudResponse
    {
        [JsonPropertyName("serialNumber")]
        public string SerialNumber { get; init; }
        [JsonPropertyName("playerName")]
        public string PlayerCode { get; init; }
        [JsonPropertyName("type")]
        public int Type { get; init; }
        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }
        [JsonPropertyName("createTime")]
        public DateTime CreateTime { get; init; }
        /// <summary>
        /// Gets or sets the state.
        /// 处理结果,
        /// 成功(0)
        /// 余额不足(10),
        /// 其他情况请参考错误讯息代码说明
        /// </summary>
        [JsonPropertyName("state")]
        public int State { get; init; }
        /// <summary>
        /// Gets or sets the balance.
        /// 余额, 只有当 state 为成功(0)时, 会显示正确余额, 其他情况都为 0
        /// </summary>
        [JsonPropertyName("balance")]
        public decimal Balance { get; init; }
    }
}
