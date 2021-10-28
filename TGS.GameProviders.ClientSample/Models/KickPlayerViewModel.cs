namespace TGS.GameProviders.ClientSample.Models
{
    public record KickPlayerViewModel
    {
        public string GameProviderCode { get; init; } = default!;
        public string BrandCode { get; init; } = default!;
        public string PlayerCode { get; init; } = default!;
        public string CurrencyCode { get; init; } = default!;
    }
}
