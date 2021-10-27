namespace TGS.GameProviders.ViewModels
{
    public class PlayerFundCheckingRequest
    {
        public string BrandCode { get; init; }

        public string CurrencyCode { get; init; }

        public string OrderId { get; set; }

        public string PlayerCode { get; init; }
    }
}
