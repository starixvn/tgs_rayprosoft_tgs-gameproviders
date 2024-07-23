namespace Tgs.GameProviders.Domains.SettingConfiguration;
public record MicroGamingConfiguration
{
	public Uri StsApiUrl { get; init; } = default!;
	public string ClientId { get; set; } = default!;
	public string ClientSecret { get; set; } = default!;
	public Uri ApiUrl { get; init; } = default!;
	public string AgentCode { get; init; } = default!;
}
