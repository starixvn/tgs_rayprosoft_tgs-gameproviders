using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.JDB.Models;

public record JDBBetHistoryResponse
{
    [JsonPropertyName("status")]
    public string Status { get; init; }

    [JsonPropertyName("err_text")]
    public string ErrorMessage { get; init; }

    [JsonPropertyName("data")]
    public JDBBetHistoryPageUrl[] Data { get; init; }
}
