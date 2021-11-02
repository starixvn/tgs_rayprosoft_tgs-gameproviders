// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.SBO.Models
{
    public record SBOUpdatePlayerBetLimitsRequest : SBOBasePlayerRequest
    {
        [JsonPropertyName("BetSettings")]
        public SBOBetLimit[] BetLimits { get; init; }
    }
}
