namespace Tgs.GameProviders.Domains.SettingConfiguration;
public record TPhereConfiguration
{
	public Uri ApiUrl { get; init; } = default!;

	public string ApiKey { get; init; } = default!;
	public string ApiToken { get; init; } = default!;
}
