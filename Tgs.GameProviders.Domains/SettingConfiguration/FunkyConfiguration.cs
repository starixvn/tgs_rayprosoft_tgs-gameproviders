namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record FunkyConfiguration
{
	public Uri ApiUrl { get; init; } = default!;
	public Uri ApiReportUrl { get; init; } = default!;
	public string Authentication { get; init; } = default!;
	public string UserAgent { get; init; } = default!;
}
