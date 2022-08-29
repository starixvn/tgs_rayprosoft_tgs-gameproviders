using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Domains.SettingConfiguration.SexyGaming;

public record SexyGamingBetLimitSexybcrt
{
    [JsonPropertyName("LIVE")]
    public SexyGamingBetLimitSexybcrtLive Live { get; init; } = default!;
}
