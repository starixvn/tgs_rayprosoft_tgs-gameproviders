namespace Tgs.GameProviders.Client;

public interface IGameProvidersBalancesClient
{
    IAsyncEnumerable<BalanceResponse> AwcAelottoGetBalances(string brandCode, CancellationToken cancellationToken = default);

    IAsyncEnumerable<BalanceResponse> AwcAwsGetBalances(string brandCode, CancellationToken cancellationToken = default);

    IAsyncEnumerable<BalanceResponse> AwcCockfightGetBalances(string brandCode, CancellationToken cancellationToken = default);

    IAsyncEnumerable<BalanceResponse> AwcFacaiGetBalances(string brandCode, CancellationToken cancellationToken = default);

    IAsyncEnumerable<BalanceResponse> AwcFsGetBalances(string brandCode, CancellationToken cancellationToken = default);

    IAsyncEnumerable<BalanceResponse> AwcKmGetBalances(string brandCode, CancellationToken cancellationToken = default);

    IAsyncEnumerable<BalanceResponse> AwcLudoGetBalances(string brandCode, CancellationToken cancellationToken = default);

    IAsyncEnumerable<BalanceResponse> AwcPgsGetBalances(string brandCode, CancellationToken cancellationToken = default);

    IAsyncEnumerable<BalanceResponse> AwcRcbGetBalances(string brandCode, CancellationToken cancellationToken = default);

    IAsyncEnumerable<BalanceResponse> AwcSgGetBalances(string brandCode, CancellationToken cancellationToken = default);

    IAsyncEnumerable<BalanceResponse> AwcSpadeGetBalances(string brandCode, CancellationToken cancellationToken = default);

    IAsyncEnumerable<BalanceResponse> AwcVenusGetBalances(string brandCode, CancellationToken cancellationToken = default);

    IAsyncEnumerable<BalanceResponse> AwcYlGetBalances(string brandCode, CancellationToken cancellationToken = default);
}
