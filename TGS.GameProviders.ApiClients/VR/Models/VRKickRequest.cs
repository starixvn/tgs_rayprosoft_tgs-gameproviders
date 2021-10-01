using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.VR.Models
{
    public record VRKickRequest
    {
        [JsonPropertyName("playerName")]
        public string PlayerCode { get; init; }
    }
}
