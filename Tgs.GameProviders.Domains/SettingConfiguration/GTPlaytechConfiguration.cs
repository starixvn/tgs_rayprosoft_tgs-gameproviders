namespace Tgs.GameProviders.Domains.SettingConfiguration;
public record GTPlaytechConfiguration
{
	public Uri ApiUrl { get; init; } = default!;
	public Uri LauncherApiUrl { get; init; } = default!;
	public Uri DataApiUrl { get; init; } = default!;
	public int BrandId { get; init; } = default!;
	public string SecretKey { get; init; } = default!;
}
