namespace TGS.GameProviders.Core.WGSports.Models
{
    public record WGSportsAccessMoneyRequest
    {
        public string PlayerCode { get; init; }
        public WGSportsAmountTypes AmountType { get; init; }
        public string OrderId { get; init; }
        public string Amount { get; init; }
    }
}
