namespace TGS.GameProviders.Client
{
    public record KickPlayerRequest
    {
        public string BrandCode { get; init; } = default!;

        public string PlayerCode { get; init; } = default!;

        public string CurrencyCode { get; init; } = default!;
    }
}
