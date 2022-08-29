// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.SBO.Models;

public record SBOUpdatePlayerBetLimitsRequest : SBOBasePlayerRequest
{
    [JsonPropertyName("BetSettings")]
    public SBOBetLimit[] BetLimits { get; init; }
}
