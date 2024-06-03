namespace Tgs.GameProviders.Core;

public record PlayerFundCheckingQuery
{
	public string PlayerCode { get; init; }
	public string OrderId { get; set; }
}
