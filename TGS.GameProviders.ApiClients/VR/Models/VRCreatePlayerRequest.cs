using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.VR.Models
{
    public record VRCreatePlayerRequest
    {
        [JsonPropertyName("playerName")]
        public string PlayerCode { get; init; }
    }
}
