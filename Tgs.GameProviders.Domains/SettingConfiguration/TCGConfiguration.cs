namespace Tgs.GameProviders.Domains.SettingConfiguration;

/// <summary>
/// TCG 設定檔案
/// </summary>
public record TCGConfiguration
{
	/// <summary>
	/// The API URL
	/// </summary>
	public Uri ApiUrl { get; init; } = default!;
	/// <summary>
	/// The game URL
	/// </summary>
	public Uri GameUrl { get; init; } = default!;
	/// <summary>
	/// The currency code
	/// </summary>
	public string CurrencyCode { get; init; } = default!;
	/// <summary>
	/// The merchant code
	/// </summary>
	public string MerchantCode { get; init; } = default!;
	/// <summary>
	/// The DES key
	/// </summary>
	public string DESKey { get; init; } = default!;
	/// <summary>
	/// The sh a256 key
	/// </summary>
	public string SHA256Key { get; init; } = default!;
}
