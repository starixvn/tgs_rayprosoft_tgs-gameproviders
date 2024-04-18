using Tgs.GameProviders.Domains.SettingConfiguration.WMCasino;

namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record WMCasinoConfiguration : PluggableConfigurationBase
{
	public Uri ApiUrl { get; init; } = default!;

	public WMCasinoBrandMerchant[] BrandMerchants { get; init; } = default!;
}
