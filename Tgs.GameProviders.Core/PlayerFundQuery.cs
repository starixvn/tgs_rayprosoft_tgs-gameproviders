namespace Tgs.GameProviders.Core;

public record PlayerFundQuery
{
    public string BrandCode { get; init; }
    public string PlayerCode { get; init; }
    public string CurrencyCode { get; init; }
    public string OrderId { get; set; }
    public decimal Amount { get; init; }
}
