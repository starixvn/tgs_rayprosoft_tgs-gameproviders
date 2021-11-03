namespace TGS.GameProviders.Core.WGHorse.Models
{
    public record WGHorseAccessMoneyRequest
    {
        public string PlayerCode { get; init; }
        public WGHorseAmountTypes AmountType { get; init; }
        public string OrderId { get; init; }
        public string Amount { get; init; }
    }
}
