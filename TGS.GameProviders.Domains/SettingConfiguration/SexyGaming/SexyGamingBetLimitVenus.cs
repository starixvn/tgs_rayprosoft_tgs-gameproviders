using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Domains.SettingConfiguration.SexyGaming;

public record SexyGamingBetLimitVenus
{
    [JsonPropertyName("LIVE")]
    public SexyGamingBetLimitVenusLive Live { get; init; } = default!;
}
