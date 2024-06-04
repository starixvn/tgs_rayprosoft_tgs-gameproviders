namespace Tgs.GameProviders.Core;

public record PlayerLogoutCommand
{
	public string PlayerCode { get; init; } = default!;
}
