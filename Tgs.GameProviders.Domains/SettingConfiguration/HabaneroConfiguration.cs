namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record HabaneroConfiguration : PluggableConfigurationBase
{
	public string ApiKey { get; init; } = default!;

	public Uri ApiUrl { get; init; } = default!;

	public Uri BetHistoryUrl { get; init; } = default!;

	public string BrandId { get; init; } = default!;
}
