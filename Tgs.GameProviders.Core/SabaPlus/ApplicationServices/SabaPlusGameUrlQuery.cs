using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.SabaPlus.ApplicationServices;

public record SabaPlusGameUrlQuery : GameUrlQuery
{
	[JsonIgnore]
	public string ProviderCode { get; set; }
};
