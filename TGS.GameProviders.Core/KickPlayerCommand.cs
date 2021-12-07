namespace TGS.GameProviders.Core
{
    public record KickPlayerCommand
    {
        public string PlayerCode { get; init; }
    }
}
