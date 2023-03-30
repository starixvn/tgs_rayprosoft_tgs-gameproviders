namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record OBetConfiguration
{
    public Uri ApiUrl { get; init; } = default!;
    public string ApiKey { get; init; } = default!;
    public string AgentId { get; init; } = default!;
}
