namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record SBOTransferConfiguration
{
	public Uri ApiUrl { get; set; } = default!;
	public string CompanyKey { get; set; } = default!;
	public string Agent { get; set; } = default!;
	public string Currency { get; set; } = default!;
	public int Min { get; set; }
	public int Max { get; set; }
	public int MaxPerMatch { get; set; }
	public int CasinoTableLimit { get; set; }
}
