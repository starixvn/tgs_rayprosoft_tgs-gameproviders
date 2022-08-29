using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.VR.Models.Commons;

public record VRErrorCodeResponse
{
    [JsonPropertyName("errorCode")]
    public int ErrorCode { get; init; }
}
