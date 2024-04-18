namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record JokerConfiguration
{
	public Uri ApiUrl { get; init; } = default!;
	public Uri GameLaunchUrl { get; init; } = default!;
	public string AppId { get; init; } = default!;
	public string SecretKey { get; init; } = default!;
}
