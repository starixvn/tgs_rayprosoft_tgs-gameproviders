namespace Tgs.GameProviders.Client;

public record PlayerFundCheckingResponse
{
	/// <summary>
	/// The order status
	/// </summary>
	public bool OrderStatus { get; init; }
}
