using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.VR.Models
{
    public record VRCreatePlayerRequest
    {
        [JsonPropertyName("playerName")]
        public string PlayerCode { get; init; }
    }
}
