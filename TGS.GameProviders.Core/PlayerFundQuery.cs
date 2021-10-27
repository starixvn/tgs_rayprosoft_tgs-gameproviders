namespace TGS.GameProviders.Core
{
    public record PlayerFundQuery
    {
        public string BrandCode { get; init; }
        public string PlayerCode { get; init; }
        public string CurrencyCode { get; init; }
        public decimal Amount { get; init; }
    }
}
