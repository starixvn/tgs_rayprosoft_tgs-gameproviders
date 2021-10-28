namespace TGS.GameProviders.Client
{
    public record PlayerFundCheckingRequest
    {
        public string BrandCode { get; init; } = default!;

        public string PlayerCode { get; init; } = default!;

        public string CurrencyCode { get; init; } = default!;

        public string OrderId { get; set; } = default!;
    }
}
