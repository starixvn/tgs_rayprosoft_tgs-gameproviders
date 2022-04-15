using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.VR.Models
{
    public record VRGetBalanceRequest
    {
        [JsonPropertyName("playerName")]
        public string PlayerCode { get; init; }
    }
}
