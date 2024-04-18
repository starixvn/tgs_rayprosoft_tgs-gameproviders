namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record KaiyuanConfiguration
{
	/// <summary>
	/// The amount maximum
	/// </summary>
	public decimal AmountMaximum { get; init; } = default!;

	/// <summary>
	/// The amount format
	/// </summary>
	public string AmountFormat { get; init; } = default!;

	/// <summary>
	/// The API URL
	/// </summary>
	public Uri ApiUrl { get; init; } = default!;

	/// <summary>
	/// The API record URL
	/// </summary>
	public Uri ApiRecordUrl { get; init; } = default!;

	/// <summary>
	/// The deskey
	/// </summary>
	public string Deskey { get; init; } = default!;

	/// <summary>
	/// The md5key
	/// </summary>
	public string Md5key { get; init; } = default!;

	/// <summary>
	/// The currency code
	/// </summary>
	public string CurrencyCode { get; init; } = default!;

	/// <summary>
	/// The agent identifier
	/// </summary>
	public string AgentId { get; init; } = default!;
}
