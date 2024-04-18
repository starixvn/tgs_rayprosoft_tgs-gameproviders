using System.Text.Json.Nodes;

namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record AwcCockfightConfiguration
{
	public Uri ApiUrl { get; init; } = default!;
	public Uri ApiRecordUrl { get; init; } = default!;
	public string AgentId { get; init; } = default!;
	public string Cert { get; init; } = default!;
	public JsonNode BetLimit { get; init; } = default!;
}
