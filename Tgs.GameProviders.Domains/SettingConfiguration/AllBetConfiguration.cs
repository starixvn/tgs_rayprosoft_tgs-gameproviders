using Tgs.GameProviders.Domains.SettingConfiguration.AllBet;

namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record AllBetConfiguration : PluggableConfigurationBase
{
	public Uri ApiUrl { get; init; } = default!;

	public AllBetBrandMerchant[] BrandMerchants { get; init; } = default!;
}
