namespace TGS.GameProviders.Client
{
    public record PlayerFundRequest
    {
        public string BrandCode { get; init; } = default!;

        public string PlayerCode { get; init; } = default!;

        public string CurrencyCode { get; init; } = default!;

        public string OrderId { get; set; } = default!;

        public decimal Amount { get; init; } = default!;
    }
}
