namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record DreamGamingTransferConfiguration
{
    public Uri ApiUrl { get; init; } = default!;
    public Uri VideoGameApiUrl { get; init; } = default!;
    public string ApiKey { get; init; } = default!;
    public string AgentName { get; init; } = default!;
    public string TableLimit { get; init; } = default!;
}
