using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.BigGaming.Models.Commons;

namespace Tgs.GameProviders.Core.BigGaming.Models
{
    public record BigGamingCreatePlayerRequest : BigGamingParamsDigestRequest
    {
        /// <summary>
        /// 登录ID.
        /// </summary>
        [JsonPropertyName("loginId")]
        public string LoginId { get; set; } = default!;
        /// <summary>
        /// 会员昵称.
        /// </summary>
        [JsonPropertyName("nickname")]
        public string Nickname { get; set; } = default!;
        /// <summary>
        /// 代理商账号，将会员账号创建到指定的代理名下.
        /// </summary>
        [JsonPropertyName("agentLoginId")]
        public string AgentLoginId { get; set; } = default!;
    }
}
