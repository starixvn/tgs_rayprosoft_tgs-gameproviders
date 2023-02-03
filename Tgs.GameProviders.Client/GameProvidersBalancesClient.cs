using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Client;

internal class GameProvidersBalancesClient : IGameProvidersBalancesClient
{
    private static readonly JsonSerializerOptions? _SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    private readonly HttpClient _httpClient;

    public GameProvidersBalancesClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async IAsyncEnumerable<BalanceResponse> AwcAelottoGetBalances(
        string brandCode,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _httpClient
                    .GetAsync(
                         requestUri: $"api/awc_aelotto/balances/{brandCode}",
                         cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content
        .ReadAsStringAsync(
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

        var balances = JsonSerializer.Deserialize<IEnumerable<BalanceResponse>>(contentJson, _SerializerOptions)
                        ?? Enumerable.Empty<BalanceResponse>();

        if (!balances.Any())
        {
            yield break;
        }

        foreach (var balance in balances)
        {
            yield return balance;
        }
    }

    public async IAsyncEnumerable<BalanceResponse> AwcAwsGetBalances(
        string brandCode,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _httpClient
                    .GetAsync(
                         requestUri: $"api/awc_aws/balances/{brandCode}",
                         cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content
        .ReadAsStringAsync(
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

        var balances = JsonSerializer.Deserialize<IEnumerable<BalanceResponse>>(contentJson, _SerializerOptions)
                        ?? Enumerable.Empty<BalanceResponse>();

        if (!balances.Any())
        {
            yield break;
        }

        foreach (var balance in balances)
        {
            yield return balance;
        }
    }
}
