using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.Avia.Models
{
    internal class AviaBalanceResponse
    {
        [JsonPropertyName("Currency")]
        public string Currency { get; init; }

        [JsonPropertyName("Money")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal Money { get; init; }
    }
}
