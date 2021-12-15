using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.WGSports.Models.Commons;

namespace TGS.GameProviders.ApiClients.WGSports.Models
{
    public record WGSportsGetBalanceResponse : WGSportsResponse
    {
        [JsonPropertyName("Balance")]
        public string Balance { get; init; } = default!;
        [JsonPropertyName("Currency")]
        public string Currency { get; init; } = default!;
    }
}
