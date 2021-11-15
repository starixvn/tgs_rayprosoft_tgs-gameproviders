namespace TGS.GameProviders.Core.WGSports.Models
{
    public record WGSportsAccessMoneyRequest
    {
        public string PlayerCode { get; init; } = default!;
        public WGSportsAmountTypes AmountType { get; init; }
        public string OrderId { get; init; } = default!;
        public string Amount { get; init; } = default!;
    }
}
