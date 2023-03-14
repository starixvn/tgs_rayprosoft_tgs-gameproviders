namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record FaChaiConfiguration
{
    public Uri ApiUrl { get; init; } = default!;
    public string AgentCode { get; init; } = default!;
    public string AgentKey { get; init; } = default!;
    public string Currency { get; init; } = default!;
}
