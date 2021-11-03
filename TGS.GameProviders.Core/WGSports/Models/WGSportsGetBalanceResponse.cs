using System.Text.Json.Serialization;
using TGS.GameProviders.Core.WGSports.Models.Commons;

namespace TGS.GameProviders.Core.WGSports.Models
{
    public record WGSportsGetBalanceResponse : WGSportsResponse
    {
        [JsonPropertyName("Balance")]
        public string Balance { get; init; }
        [JsonPropertyName("Currency")]
        public string Currency { get; init; }
    }
}
