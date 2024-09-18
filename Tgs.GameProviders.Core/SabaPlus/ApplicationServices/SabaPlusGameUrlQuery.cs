namespace Tgs.GameProviders.Core.SabaPlus.ApplicationServices;

public record SabaPlusExtraInfo
{
	public string BrowserUserAgent { get; set; }
}

public record SabaPlusGameUrlQuery : GameUrlQuery
{
	public new SabaPlusExtraInfo ExtraInfo { get; init; }
};
