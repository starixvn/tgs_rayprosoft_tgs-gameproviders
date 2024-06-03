namespace Tgs.GameProviders.Domains.SettingConfiguration;

/// <summary>
/// BG 設定檔案
/// </summary>
public record BigGamingConfiguration
{
	public Uri ApiUrl { get; init; } = default!;

	public bool Status { get; init; }

	public string CurrencyCode { get; init; } = default!;

	public string AgentLoginIdPassword { get; init; } = default!;

	public string SecretKey { get; init; } = default!;

	public string SecretCode { get; init; } = default!;
	public OppositeBetSetting[] OppositeBets { get; init; } = default!;
}
