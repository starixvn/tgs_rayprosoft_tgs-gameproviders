using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.WGSports.Models.Commons
{
    public record WGSportsResponse
    {
        [JsonPropertyName("ReturnCode")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int ReturnCode { get; init; }
    }
}
