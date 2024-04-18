using Tgs.GameProviders.Domains.SettingConfiguration.TCG;

namespace Tgs.GameProviders.Domains.SettingConfiguration;

/// <summary>
/// TCG 設定檔案
/// </summary>
public record TCGConfiguration : PluggableConfigurationBase
{
	/// <summary>
	/// The API URL
	/// </summary>
	public Uri ApiUrl { get; init; } = default!;
	/// <summary>
	/// The merchants
	/// </summary>
	public TCGMerchant[] Merchants { get; init; } = default!;
}
