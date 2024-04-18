namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record TFGamingTransferConfiguration
{
	public Uri ApiUrl { get; init; } = default!;

	public string OperatorId { get; init; } = default!;
	public string PrivateToken { get; init; } = default!;
}
