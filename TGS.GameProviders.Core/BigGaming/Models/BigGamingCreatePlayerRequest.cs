using System.Text.Json.Serialization;
using TGS.GameProviders.Core.BigGaming.Models.Commons;

namespace TGS.GameProviders.Core.BigGaming.Models
{
    public record BigGamingCreatePlayerRequest : BigGamingParamsDigestRequest
    {
        /// <summary>
        /// 登录ID.
        /// </summary>
        [JsonPropertyName("loginId")]
        public string LoginId { get; set; }
        /// <summary>
        /// 会员昵称.
        /// </summary>
        [JsonPropertyName("nickname")]
        public string Nickname { get; set; }
        /// <summary>
        /// 代理商账号，将会员账号创建到指定的代理名下.
        /// </summary>
        [JsonPropertyName("agentLoginId")]
        public string AgentLoginId { get; set; }
    }
}
