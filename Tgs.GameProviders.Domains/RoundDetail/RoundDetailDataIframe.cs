using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Domains.RoundDetail;

public record RoundDetailDataIframe : RoundDetailDataBase
{
	public RoundDetailDataIframe() : base(RoundDetailDataType.Iframe)
	{
	}

	[JsonPropertyName("value")]
	public Uri? Value { get; init; } = default!;
}
