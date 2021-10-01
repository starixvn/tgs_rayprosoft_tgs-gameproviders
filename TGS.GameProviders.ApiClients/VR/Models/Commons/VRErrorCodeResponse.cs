using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.VR.Models.Commons
{
    public record VRErrorCodeResponse
    {
        [JsonPropertyName("errorCode")]
        public int ErrorCode { get; init; }
    }
}
