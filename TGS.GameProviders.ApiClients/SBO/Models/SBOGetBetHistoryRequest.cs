// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.SBO.Models
{
    public record SBOGetBetHistoryRequest : SBOBaseRequest
    {
        [JsonPropertyName("EndDate")]
        public string EndDate { get; init; }

        [JsonPropertyName("Portfolio")]
        public string Portfolio { get; init; }

        [JsonPropertyName("StartDate")]
        public string StartDate { get; init; }
    }
}
