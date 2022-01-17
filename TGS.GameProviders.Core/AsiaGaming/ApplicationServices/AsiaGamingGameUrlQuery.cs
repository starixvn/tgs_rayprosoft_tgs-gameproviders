namespace TGS.GameProviders.Core.AsiaGaming.ApplicationServices;

public record AsiaGamingGameUrlQuery : GameUrlQuery
{
    public decimal PlayerBalance { get; init; }
}