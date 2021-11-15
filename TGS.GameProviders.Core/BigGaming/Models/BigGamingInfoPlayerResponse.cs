using System;
using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.BigGaming.Models
{
    public record BigGamingInfoPlayerResponse
    {
        [JsonPropertyName("sn")]
        public string Sn { get; set; } = default!;
        [JsonPropertyName("userId")]
        public long UserId { get; set; } = default!;
        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;
        [JsonPropertyName("nickname")]
        public string Nickname { get; set; } = default!;
        [JsonPropertyName("regTime")]
        public string RegTime { get; set; } = default!;
        [JsonPropertyName("currency")]
        public int Currency { get; set; }
        [JsonPropertyName("userStatus")]
        public int UserStatus { get; set; }
        [JsonPropertyName("memo")]
        public string Memo { get; set; } = default!;
        [JsonPropertyName("loginId")]
        public string LoginId { get; set; } = default!;
        [JsonPropertyName("status")]
        public int Status { get; set; }
        [JsonPropertyName("regType")]
        public string RegType { get; set; } = default!;
        [JsonPropertyName("regIp")]
        public string RegIp { get; set; } = default!;
        [JsonPropertyName("loginLastUpdateTime")]
        public DateTime LoginLastUpdateTime { get; set; }
        [JsonPropertyName("loginIp")]
        public string LoginIp { get; set; } = default!;
        [JsonPropertyName("loginCount")]
        public int LoginCount { get; set; }
        [JsonPropertyName("remark")]
        public string Remark { get; set; } = default!;
    }
}
