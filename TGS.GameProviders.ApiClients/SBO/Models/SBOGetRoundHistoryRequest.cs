// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.SBO.Models
{
    public record SBOGetRoundHistoryRequest : SBOBaseRequest
    {
        [JsonPropertyName("RefNos")]
        public string ExternalRoundId { get; init; }

        [JsonPropertyName("Language")]
        public string Language { get; init; }

        [JsonPropertyName("Portfolio")]
        public string Portfolio { get; init; }
    }
}
