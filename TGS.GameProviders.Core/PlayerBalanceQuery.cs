namespace TGS.GameProviders.Core
{
    public record PlayerBalanceQuery
    {
        public string BrandCode { get; init; }
        public string PlayerCode { get; init; }
        public string CurrencyCode { get; init; }
    }
}
