using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.VR.Models
{
    public record VRGetBalanceRequest
    {
        [JsonPropertyName("playerName")]
        public string PlayerCode { get; init; }
    }
}
