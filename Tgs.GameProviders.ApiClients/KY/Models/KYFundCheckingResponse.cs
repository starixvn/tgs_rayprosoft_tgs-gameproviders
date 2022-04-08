using System.Text.Json.Serialization;
using Tgs.GameProviders.ApiClients.KY.Models.Commons;

namespace Tgs.GameProviders.ApiClients.KY.Models
{
    public record KYFundCheckingResponse : KYBaseDataResponse
    {
        /// <summary>
        /// The status
        /// 状态码（-1:不存在、0:成功、2:失败、3:处理中）
        /// </summary>
        [JsonPropertyName("status")]
        public int Status { get; init; }

        [JsonPropertyName("money")]
        public decimal? Amount { get; init; }
    }
}
