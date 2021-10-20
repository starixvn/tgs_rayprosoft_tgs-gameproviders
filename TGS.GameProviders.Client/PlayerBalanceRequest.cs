namespace TGS.GameProviders.Client
{
    public record PlayerBalanceRequest
    {
        public string GameProviderCode { get; init; }
        public string BrandCode { get; init; }
        public string PlayerCode { get; init; }
        public string CurrencyCode { get; init; }
    }
}
