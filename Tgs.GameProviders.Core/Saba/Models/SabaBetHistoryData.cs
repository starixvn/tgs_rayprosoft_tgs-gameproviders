using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Saba.Models;

public record SabaBetHistoryData
{
    [JsonPropertyName("last_version_key")]
    public long VersionKey { get; init; }

    [JsonPropertyName("BetDetails")]
    public SabaGameResult[] Results { get; init; }
}
