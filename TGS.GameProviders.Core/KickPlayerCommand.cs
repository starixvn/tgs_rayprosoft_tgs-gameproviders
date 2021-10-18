namespace TGS.GameProviders.Core
{
    public record KickPlayerCommand
    {
        public string BrandCode { get; init; }
        public string PlayerCode { get; init; }
        public string CurrencyCode { get; init; }
    }
}
