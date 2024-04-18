namespace Tgs.GameProviders.Core.SM01.ApplicationServices;

public record SM01PlatformLoginQuery
{
	public string PlayerCode { get; init; } = default!;
}
