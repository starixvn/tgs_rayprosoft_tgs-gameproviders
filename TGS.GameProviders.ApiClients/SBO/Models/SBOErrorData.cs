using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.SBO.Models
{
    public record SBOErrorData
    {
        [JsonPropertyName("id")]
        public SBOApiErrorCode Code { get; init; }

        [JsonPropertyName("msg")]
        public string Message { get; init; }
    }
}
