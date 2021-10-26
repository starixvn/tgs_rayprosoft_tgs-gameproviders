namespace TGS.GameProviders.ClientSample.Models
{
    public record KickPlayerViewModel
    {
        public string GameProviderCode { get; init; }
        public string BrandCode { get; init; }
        public string PlayerCode { get; init; }
        public string CurrencyCode { get; init; }
    }
}
