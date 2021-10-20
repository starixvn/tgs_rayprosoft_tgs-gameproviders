namespace TGS.GameProviders.ViewModels
{
    public record PlayerBalanceRequest
    {
        public string BrandCode { get; init; }
        public string PlayerCode { get; init; }
        public string CurrencyCode { get; init; }
    }
}
