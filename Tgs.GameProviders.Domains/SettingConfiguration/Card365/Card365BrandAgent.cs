namespace Tgs.GameProviders.Domains.SettingConfiguration.Card365;

public record Card365BrandAgent
{
	public string BrandCode { get; init; } = default!;
	public string AgentId { get; init; } = default!;
}
