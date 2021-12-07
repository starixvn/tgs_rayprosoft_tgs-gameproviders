using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.WGHorse.Models.Commons
{
    public record WGHorseResponse
    {
        [JsonPropertyName("ReturnCode")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int ReturnCode { get; init; }
    }
}
