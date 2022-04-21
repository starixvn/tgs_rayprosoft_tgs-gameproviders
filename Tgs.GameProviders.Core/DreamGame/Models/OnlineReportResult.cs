using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.DreamGame.Models
{
    public record OnlineReportResult
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = default!;
        [JsonPropertyName("nickname")]
        public string Nickname { get; set; } = default!;
        [JsonPropertyName("currencyName")]
        public string CurrencyName { get; set; } = default!;
        [JsonPropertyName("ip")]
        public string Ip { get; set; } = default!;
        [JsonPropertyName("device")]
        public string Device { get; set; } = default!;
        [JsonPropertyName("login")]
        public string Login { get; set; } = default!;
        [JsonPropertyName("memberId")]
        public long MemberId { get; set; }
    }
}
