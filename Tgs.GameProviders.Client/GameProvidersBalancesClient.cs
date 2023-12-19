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

    public async IAsyncEnumerable<BalanceResponse> AwcAelottoGetBalances(string brandCode, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(
            requestUri: $"api/awc_aelotto/{brandCode}/player/balances",
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

    public async IAsyncEnumerable<BalanceResponse> AwcAwsGetBalances(string brandCode, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(
            requestUri: $"api/awc_aws/{brandCode}/player/balances",
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

    public async IAsyncEnumerable<BalanceResponse> AwcCockfightGetBalances(string brandCode, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(
            requestUri: $"api/awc_cockfight/{brandCode}/player/balances",
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

    public async IAsyncEnumerable<BalanceResponse> AwcFacaiGetBalances(string brandCode, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(
            requestUri: $"api/awc_facai/{brandCode}/player/balances",
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

    public async IAsyncEnumerable<BalanceResponse> AwcFsGetBalances(string brandCode, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(
            requestUri: $"api/awc_fs/{brandCode}/player/balances",
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

    public async IAsyncEnumerable<BalanceResponse> AwcKmGetBalances(string brandCode, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(
            requestUri: $"api/awc_km/{brandCode}/player/balances",
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

    public async IAsyncEnumerable<BalanceResponse> AwcLudoGetBalances(string brandCode, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(
            requestUri: $"api/awc_ludo/{brandCode}/player/balances",
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

    public async IAsyncEnumerable<BalanceResponse> AwcPgsGetBalances(string brandCode, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(
            requestUri: $"api/awc_pgs/{brandCode}/player/balances",
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

    public async IAsyncEnumerable<BalanceResponse> AwcRcbGetBalances(string brandCode, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(
            requestUri: $"api/awc_rcb/{brandCode}/player/balances",
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

    public async IAsyncEnumerable<BalanceResponse> AwcSgGetBalances(string brandCode, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(
            requestUri: $"api/awc_sg/{brandCode}/player/balances",
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

    public async IAsyncEnumerable<BalanceResponse> AwcSpadeGetBalances(string brandCode, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(
            requestUri: $"api/awc_spade/{brandCode}/player/balances",
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

    public async IAsyncEnumerable<BalanceResponse> AwcVenusGetBalances(string brandCode, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(
            requestUri: $"api/awc_venus/{brandCode}/player/balances",
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

    public async IAsyncEnumerable<BalanceResponse> AwcYlGetBalances(string brandCode, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(
            requestUri: $"api/awc_yl/{brandCode}/player/balances",
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
