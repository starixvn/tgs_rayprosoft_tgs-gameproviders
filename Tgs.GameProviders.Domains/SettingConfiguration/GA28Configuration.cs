namespace Tgs.GameProviders.Domains.SettingConfiguration;
public record GA28Configuration
{
	public Uri ApiUrl { get; init; } = default!;

	public string PrivateKey { get; init; } = default!;
	public string PartnerAccountId { get; init; } = default!;
}
