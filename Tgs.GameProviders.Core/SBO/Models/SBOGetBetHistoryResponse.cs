// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.SBO.Models
{
    public record SBOGetBetHistoryResponse : SBOBaseResponse
    {
        [JsonPropertyName("result")]
        public SBOGameResult[] Results { get; init; }
    }
}
