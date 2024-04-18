using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Domains.RoundDetail;

[JsonConverter(typeof(RoundDetailDataConverter))]
public abstract record RoundDetailDataBase
{
	protected RoundDetailDataBase(RoundDetailDataType type)
	{
		Type = type.ToString();
	}

	[JsonPropertyName("type")]
	public string Type { get; }
}
