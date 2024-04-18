using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record EvolutionTransferConfiguration
{
	public Uri ApiUrl { get; init; } = default!;
	[JsonPropertyName("CasinoID")]
	public string CasinoId { get; init; } = default!;
	public string Ua2Token { get; init; } = default!;
	public string GameHistoryApiToken { get; init; } = default!;
	public string ExternalLobbyApiToken { get; init; } = default!;
	public Uri Ua2AuthenticationUrl { get; init; } = default!;
	[JsonPropertyName("ecID")]
	public string EcId { get; init; } = default!;
}
