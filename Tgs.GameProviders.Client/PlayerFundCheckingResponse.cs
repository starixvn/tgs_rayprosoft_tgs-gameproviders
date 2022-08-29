namespace Tgs.GameProviders.Client;

internal record PlayerFundCheckingResponse
{
    /// <summary>
    /// The order status
    /// </summary>
    public bool OrderStatus { get; init; }
}
