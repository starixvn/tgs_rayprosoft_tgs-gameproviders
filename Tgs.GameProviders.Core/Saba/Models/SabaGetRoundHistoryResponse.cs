using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Saba.Models;

public record SabaGetRoundHistoryResponse
{
    [JsonPropertyName("error_code")]
    public SabaGetRoundHistoryApiError ErrorCode { get; init; }

    [JsonPropertyName("message")]
    public string ErrorMessage { get; init; }

    [JsonPropertyName("Data")]
    public SabaBetHistoryData Data { get; init; }
}
