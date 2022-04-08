namespace Tgs.GameProviders.ApiClients.WGHorse.Models
{
    public record WGHorseAccessMoneyRequest
    {
        public string PlayerCode { get; init; } = default!;
        public WGHorseAmountTypes AmountType { get; init; }
        public string OrderId { get; init; } = default!;
        public string Amount { get; init; } = default!;
    }
}
