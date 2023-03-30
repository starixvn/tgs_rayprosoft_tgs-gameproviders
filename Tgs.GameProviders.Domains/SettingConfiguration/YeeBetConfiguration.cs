namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record YeeBetConfiguration
{
    public Uri ApiUrl { get; init; } = default!;
    public string AppId { get; init; } = default!;
    public string SecretKey { get; init; } = default!;
    public string? SetCurrency { get; init; }
}
