using System.Text.Json.Serialization;
using Tgs.GameProviders.ApiClients.LY.Models.Commons;

namespace Tgs.GameProviders.ApiClients.LY.Models;

public record LYFundCheckingResponse : LYBaseDataResponse
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
