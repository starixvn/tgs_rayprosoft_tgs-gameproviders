using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.SabaPlus.ApplicationServices;

public record SabaPlusExtraInfo
{
	public string BrowserUserAgent { get; set; }
}

public record SabaPlusGameUrlQuery : GameUrlQuery
{
	[JsonIgnore]
	public string ProviderCode { get; set; }

	public new SabaPlusExtraInfo ExtraInfo { get; init; }
};
