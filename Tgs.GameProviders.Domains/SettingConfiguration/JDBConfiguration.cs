namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record JDBConfiguration
{
	public Uri ApiUrl { get; init; } = default!;

	public bool MoreGame { get; init; }

	public string AESIv { get; init; } = default!;

	public string AESKey { get; init; } = default!;

	public string ClientDomain { get; init; } = default!;

	public int TimezoneHour { get; init; }

	public string CurrencyCode { get; init; } = default!;

	public string Id { get; init; } = default!;
}
