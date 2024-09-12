namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record SabaPlusSubProviderConfiguration
{
	public string Code { get; set; } = default!;
	public Uri ApiUrl { get; set; } = default!;
	public string OperatorPassword { get; set; } = default!;
	public string OperatorSignKey { get; set; } = default!;
	public string AgentId { get; set; } = default!;
	public string AgentKey { get; set; } = default!;
	public string AgentCode { get; set; } = default!;
	public string AgentExtraInfo { get; set; } = default!;
	public object Additional { get; set; } = default!;
	public string GameType { get; set; } = default!;
}

public record SabaPlusConfiguration
{
	public List<SabaPlusSubProviderConfiguration> SubProviders { get; set; } = [];

	public SabaPlusSubProviderConfiguration? GetSubProviderConfiguration(string code)
		=> SubProviders.FirstOrDefault(s => string.Equals(s.Code, code, StringComparison.OrdinalIgnoreCase));
}
