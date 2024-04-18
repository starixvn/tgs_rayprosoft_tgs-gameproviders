using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Domains.RoundDetail;

public record RoundDetailResult
{
	[JsonPropertyName("data")]
	public RoundDetailDataBase[]? Data { get; init; }
}
