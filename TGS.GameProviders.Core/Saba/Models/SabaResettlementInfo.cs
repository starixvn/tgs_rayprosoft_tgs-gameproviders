using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.Saba.Models
{
    public record SabaResettlementInfo
    {
        [JsonPropertyName("winlost")]
        public decimal Winlost { get; init; }

        [JsonPropertyName("ActionDate")]
        public string ActionDate { get; init; }

        [JsonPropertyName("ticket_status")]
        public string TicketStatus { get; init; }

        [JsonPropertyName("balancechange")]
        public bool BalanceChange { get; init; }
    }
}