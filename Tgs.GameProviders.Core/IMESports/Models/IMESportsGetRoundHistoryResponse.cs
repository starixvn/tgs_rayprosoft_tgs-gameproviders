// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.IMESports.Models
{
    public record IMESportsGetRoundHistoryResponse
    {
        [JsonPropertyName("Code")]
        public IMESportsGameApiError ErrorCode { get; init; }

        [JsonPropertyName("Message")]
        public string Message { get; init; }

        [JsonPropertyName("Result")]
        public IMESportsHistoryResult[] Results { get; init; }

        [JsonPropertyName("Pagination")]
        public IMESportsPagination Pagination { get; init; }
    }
}
