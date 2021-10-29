using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.SBO.Models
{
    public record ErrorData
    {
        [JsonPropertyName("id")]
        public SBOApiErrorCode Code { get; init; }

        [JsonPropertyName("msg")]
        public string Message { get; init; }
    }
}