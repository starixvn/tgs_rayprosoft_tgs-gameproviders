using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.BigGaming.Models.Commons;

namespace Tgs.GameProviders.Core.BigGaming.Models;

public record BigGamingKickPlayerRequest : BigGamingParamsDigestRequest
{
    /// <summary>
    /// 登录ID.
    /// </summary>
    [JsonPropertyName("loginId")]
    public string LoginId { get; init; } = default!;
}
