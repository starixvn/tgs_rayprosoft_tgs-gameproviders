using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.JDB.Models;

public record JDBBetHistoryPageUrl
{
    [JsonPropertyName("path")]
    public string Path { get; init; }
}
