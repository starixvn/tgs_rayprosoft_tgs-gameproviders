namespace Tgs.GameProviders.Domains.SettingConfiguration;
public record CQ9GamingConfiguration
{
	public Uri ApiUrl { get; init; } = default!;
	public string ApiToken { get; init; } = default!;
}
