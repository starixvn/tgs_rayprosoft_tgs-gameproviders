namespace TGS.GameProviders.Client
{
    public record PlayerBalanceRequest
    {
        public string BrandCode { get; init; } = default!;

        public string PlayerCode { get; init; } = default!;

        public string CurrencyCode { get; init; } = default!;
    }
}
