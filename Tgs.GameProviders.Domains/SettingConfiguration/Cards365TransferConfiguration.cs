namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record Cards365TransferConfiguration
{
	public Uri ApiUrl { get; init; } = default!;
	public string Website { get; init; } = default!;
	public string Cert { get; init; } = default!;
	public string Extension1 { get; init; } = default!;
}
