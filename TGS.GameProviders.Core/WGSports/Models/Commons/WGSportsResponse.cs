using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.WGSports.Models.Commons
{
    public record WGSportsResponse
    {
        [JsonPropertyName("ReturnCode")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int ReturnCode { get; init; }
    }
}
