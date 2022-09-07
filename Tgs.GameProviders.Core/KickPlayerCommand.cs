namespace Tgs.GameProviders.Core;

public record KickPlayerCommand
{
    public string PlayerCode { get; init; }
}
