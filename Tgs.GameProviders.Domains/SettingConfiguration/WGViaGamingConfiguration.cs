namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record WGViaGamingConfiguration
{
	/// <summary>
	/// The amount maximum
	/// </summary>
	public decimal AmountMaximum { get; init; }

	/// <summary>
	/// The amount format
	/// </summary>
	public string AmountFormat { get; init; } = default!;

	/// <summary>
	/// The API URL
	/// </summary>
	public Uri ApiUrl { get; init; } = default!;

	/// <summary>
	/// The partner code
	/// </summary>
	public string PartnerCode { get; init; } = default!;

	/// <summary>
	/// The partner key
	/// </summary>
	public string PartnerKey { get; init; } = default!;

	/// <summary>
	/// The iv
	/// </summary>
	public string IV { get; init; } = default!;
}
