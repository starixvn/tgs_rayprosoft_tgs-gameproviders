namespace TGS.GameProviders.Client
{
    public record KickPlayerRequest
    {
        public string BrandCode { get; init; }
        public string PlayerCode { get; init; }
        public string CurrencyCode { get; init; }
    }
}
