namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record IDNLiveConfiguration
{
	public Uri ApiUrl { get; init; } = default!;

	public int OperatorId { get; init; } = default!;

	public string ApiKey { get; init; } = default!;
}
