namespace Tgs.GameProviders.Client;

public interface IGameProvidersBalancesClient
{
    public IAsyncEnumerable<BalanceResponse> AwcAelottoGetBalances(
        string brandCode,
        CancellationToken cancellationToken = default);

    public IAsyncEnumerable<BalanceResponse> AwcAwsGetBalances(
            string brandCode,
        CancellationToken cancellationToken = default);
}
