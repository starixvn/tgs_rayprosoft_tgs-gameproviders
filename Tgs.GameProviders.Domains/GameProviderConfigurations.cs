namespace Tgs.GameProviders.Domains;

public static class GameProviderConfigurations
{
	public static readonly string _template = "{0}Configuration";

	public static readonly string SabaSW = string.Format(_template, GameProviderType.SabaSW);
	public static readonly string SabaPlus = string.Format(_template, GameProviderType.SabaPlus);
}
