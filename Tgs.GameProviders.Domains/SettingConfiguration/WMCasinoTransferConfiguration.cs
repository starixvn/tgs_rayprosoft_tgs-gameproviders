namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record WMCasinoTransferConfiguration
{
	public Uri ApiUrl { get; init; } = default!;

	public string VendorId { get; init; } = default!;
	public string Signature { get; init; } = default!;
}
