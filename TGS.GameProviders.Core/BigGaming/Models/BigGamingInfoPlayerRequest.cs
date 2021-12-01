using System.Text.Json.Serialization;
using TGS.GameProviders.Core.BigGaming.Models.Commons;

namespace TGS.GameProviders.Core.BigGaming.Models
{
    public record BigGamingInfoPlayerRequest : BigGamingParamsDigestRequest
    {
        /// <summary>
        /// 登录ID.
        /// </summary>
        [JsonPropertyName("loginId")]
        public string LoginId { get; init; } = default!;
    }
}
