namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record CrPatiConfiguration
{
	public Uri ApiUrl { get; init; } = default!;
	public string ApiKey { get; init; } = default!;
	public string ApiSecret { get; init; } = default!;
}
