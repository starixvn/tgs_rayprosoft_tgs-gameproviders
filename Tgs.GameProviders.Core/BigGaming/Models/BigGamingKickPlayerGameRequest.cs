using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.BigGaming.Models.Commons;

namespace Tgs.GameProviders.Core.BigGaming.Models;

public record BigGamingKickPlayerGameRequest : BigGamingParamsDigestRequest
{
    /// <summary>
    /// 登录ID.
    /// </summary>
    [JsonPropertyName("loginId")]
    public string LoginId { get; init; } = default!;
    /// <summary>
    /// 105:捕鱼大师(默认); 411:西游捕鱼; 484:大仙捕鱼; 443:棋牌; 362:竞彩
    /// </summary>
    [JsonPropertyName("gameType")]
    public string GameType { get; init; } = default!;
}
