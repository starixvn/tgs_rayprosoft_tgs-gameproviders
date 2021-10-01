namespace TGS.GameProviders.ApiClients.WGSports.Models
{
    public record WGSportsAccessMoneyRequest
    {
        public string PlayerCode { get; init; }
        public AmountTypes AmountType { get; init; }
        public string OrderId { get; init; }
        public string Amount { get; init; }
    }
}
