using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Client;

internal class GameProvidersClient : IGameProvidersClient
{
    private static readonly string _GameURL = "/api/{0}/gameurl";
    private static readonly string _KickPlayerPath = "/api/{0}/KickPlayer/{1}";
    private static readonly string _PlayerBalancePath = "/api/{0}/balance/{1}";
    private static readonly string _PlayerFundChecking = "/api/{0}/PlayerFundChecking";
    private static readonly string _PlayerFundInPath = "/api/{0}/PlayerFundIn";
    private static readonly string _PlayerFundOutPath = "/api/{0}/PlayerFundOut";
    private static readonly string _RoundDetail = "/api/{0}/RoundDetail/{1}?language={2}";

    private static readonly JsonSerializerOptions? _SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    private readonly HttpClient _httpClient;

    public GameProvidersClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async ValueTask<SM01PlatformLoginResponse?> GamePlatformLoginBySM01Async(
        string playerCode,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            PlayerCode = playerCode,
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
           .PostAsync(
               requestUri: string.Format("/api/{0}/gameurl", GameProviderType.SM01),
               content: requestContent,
               cancellationToken: cancellationToken).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);        

        return string.IsNullOrEmpty(contentJson)
            ? null
            : JsonSerializer.Deserialize<SM01PlatformLoginResponse>(contentJson, _SerializerOptions);
    }

    public async ValueTask<Uri?> GameUrlAsync(
        string gameProviderCode,
        GameProvidersClientGameType gameType,
        int gameId,
        string brandCode,
        string playerCode,
        string currencyCode,
        Guid loginRecordId,
        GameProvidersClientPlatformType platformType,
        string language,
        string ipAddress,
        Uri? backUrl,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            GameType = gameType,
            GameId = gameId,
            BrandCode = brandCode,
            PlayerCode = playerCode,
            CurrencyCode = currencyCode,
            LoginRecordId = loginRecordId,
            PlatformType = platformType,
            Language = language,
            IPAddress = ipAddress,
            BackUrl = backUrl
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
           .PostAsync(
               requestUri: string.Format(_GameURL, gameProviderCode),
               content: requestContent,
               cancellationToken: cancellationToken).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return string.IsNullOrEmpty(contentJson) ? null : JsonSerializer.Deserialize<GameUrlResponse>(contentJson, _SerializerOptions)?.Url;
    }

    public async ValueTask<Uri?> GameUrlByAsiaGamingAsync(
        GameProvidersClientGameType gameType,
        int gameId,
        string brandCode,
        string playerCode,
        string currencyCode,
        Guid loginRecordId,
        GameProvidersClientPlatformType platformType,
        string language,
        string ipAddress,
        Uri? backUrl,
        decimal playerBalance,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            GameType = gameType,
            GameId = gameId,
            BrandCode = brandCode,
            PlayerCode = playerCode,
            CurrencyCode = currencyCode,
            LoginRecordId = loginRecordId,
            PlatformType = platformType,
            Language = language,
            IPAddress = ipAddress,
            BackUrl = backUrl,
            PlayerBalance = playerBalance
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PostAsync(
                requestUri: string.Format(_GameURL, GameProviderType.AsiaGaming),
                content: requestContent,
                cancellationToken: cancellationToken).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return string.IsNullOrEmpty(contentJson) ? null : JsonSerializer.Deserialize<GameUrlResponse>(contentJson, _SerializerOptions)?.Url;
    }

    public async ValueTask<Uri?> GameUrlByBetConstructAsync(
        GameProvidersClientGameType gameType,
        int gameId,
        string brandCode,
        string playerCode,
        string currencyCode,
        Guid loginRecordId,
        GameProvidersClientPlatformType platformType,
        string language,
        string ipAddress,
        Uri? backUrl,
        string lobbyDomain,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            GameType = gameType,
            GameId = gameId,
            BrandCode = brandCode,
            PlayerCode = playerCode,
            CurrencyCode = currencyCode,
            LoginRecordId = loginRecordId,
            PlatformType = platformType,
            Language = language,
            IPAddress = ipAddress,
            BackUrl = backUrl,
            LobbyDomain = lobbyDomain
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
           .PostAsync(
               requestUri: string.Format(_GameURL, GameProviderType.BetConstruct),
               content: requestContent,
               cancellationToken: cancellationToken).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return string.IsNullOrEmpty(contentJson) ? null : JsonSerializer.Deserialize<GameUrlResponse>(contentJson, _SerializerOptions)?.Url;
    }

    public async ValueTask KickPlayerAsync(string gameProviderCode, string playerCode, CancellationToken cancellationToken = default)
    {
        _ = await _httpClient
          .DeleteAsync(
              requestUri: string.Format(_KickPlayerPath, gameProviderCode, playerCode),
              cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<decimal?> PlayerBalanceAsync(string gameProviderCode, string playerCode, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient
            .GetAsync(
                requestUri: string.Format(_PlayerBalancePath, gameProviderCode, playerCode),
                cancellationToken: cancellationToken).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<decimal?>(contentJson);
    }

    public async ValueTask<bool?> PlayerFundCheckingAsync(string gameProviderCode, string brandCode, string playerCode, string currencyCode, string orderId, CancellationToken cancellationToken = default)
    {
        var request = new
        {
            BrandCode = brandCode,
            PlayerCode = playerCode,
            CurrencyCode = currencyCode,
            OrderId = orderId,
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
           .PostAsync(
               requestUri: string.Format(_PlayerFundChecking, gameProviderCode),
               content: requestContent,
               cancellationToken: cancellationToken).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return string.IsNullOrEmpty(contentJson) ? false : JsonSerializer.Deserialize<PlayerFundCheckingResponse>(contentJson, _SerializerOptions)?.OrderStatus;
    }

    public async ValueTask<bool?> PlayerFundCheckingByVRAsync(string brandCode, string playerCode, string currencyCode, string orderId, DateTimeOffset startTime, DateTimeOffset endTime, CancellationToken cancellationToken = default)
    {
        var request = new
        {
            BrandCode = brandCode,
            PlayerCode = playerCode,
            CurrencyCode = currencyCode,
            OrderId = orderId,
            StartTime = startTime,
            EndTime = endTime
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
           .PostAsync(
               requestUri: string.Format(_PlayerFundChecking, "VR"),
               content: requestContent,
               cancellationToken: cancellationToken).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return string.IsNullOrEmpty(contentJson) ? false : JsonSerializer.Deserialize<PlayerFundCheckingResponse>(contentJson, _SerializerOptions)?.OrderStatus;
    }

    public async ValueTask<string?> PlayerFundInAsync(string gameProviderCode, string brandCode, string playerCode, string currencyCode, string orderId, decimal amount, CancellationToken cancellationToken = default)
    {
        var request = new
        {
            BrandCode = brandCode,
            PlayerCode = playerCode,
            CurrencyCode = currencyCode,
            OrderId = orderId,
            Amount = amount
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
           .PostAsync(
               requestUri: string.Format(_PlayerFundInPath, gameProviderCode),
               content: requestContent,
               cancellationToken: cancellationToken).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return string.IsNullOrEmpty(contentJson) ? null : JsonSerializer.Deserialize<PlayerFundResponse>(contentJson, _SerializerOptions)?.OrderId;
    }

    public async ValueTask<string?> PlayerFundOutAsync(string gameProviderCode, string brandCode, string playerCode, string currencyCode, string orderId, decimal amount, CancellationToken cancellationToken = default)
    {
        var request = new
        {
            BrandCode = brandCode,
            PlayerCode = playerCode,
            CurrencyCode = currencyCode,
            OrderId = orderId,
            Amount = amount
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
           .PostAsync(
               requestUri: string.Format(_PlayerFundOutPath, gameProviderCode),
               content: requestContent,
               cancellationToken: cancellationToken).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return string.IsNullOrEmpty(contentJson) ? null : JsonSerializer.Deserialize<PlayerFundResponse>(contentJson, _SerializerOptions)?.OrderId;
    }

    public async ValueTask<RoundDetailResult?> RoundDetailAsync(string gameProviderCode, Guid roundId, string language, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient
           .GetAsync(
               requestUri: string.Format(_RoundDetail, gameProviderCode, roundId, language),
               cancellationToken: cancellationToken).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return string.IsNullOrEmpty(contentJson) ? null : JsonSerializer.Deserialize<RoundDetailResult>(contentJson, _SerializerOptions);
    }
}
