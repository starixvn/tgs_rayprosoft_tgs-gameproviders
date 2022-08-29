using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Domains.SettingConfiguration.SexyGaming;

public record SexyGamingBetLimitSexybcrtLive
{
    [JsonPropertyName("limitId")]
    public int[] LimitId { get; init; } = default!;
}
