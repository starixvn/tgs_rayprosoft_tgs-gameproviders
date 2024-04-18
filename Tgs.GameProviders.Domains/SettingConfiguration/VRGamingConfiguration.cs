namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record VRGamingConfiguration
{
	/// <summary>
	/// The API URL
	/// </summary>
	public Uri ApiUrl { get; init; } = default!;
	/// <summary>
	/// The version
	/// </summary>
	public string Version { get; init; } = "1.0";
	/// <summary>
	/// The merchant identifier
	/// </summary>
	public string MerchantId { get; init; } = default!;
	/// <summary>
	/// The deskey
	/// </summary>
	public string Deskey { get; init; } = default!;
	/// <summary>
	/// The amount maximum
	/// </summary>
	public decimal AmountMaximum { get; init; } = 3000000m;
	/// <summary>
	/// The amount format
	/// </summary>
	public string AmountFormat { get; init; } = "10,3";
}
