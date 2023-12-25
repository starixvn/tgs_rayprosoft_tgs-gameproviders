using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.RoundDetail;

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

    public async ValueTask<SM01PlatformLoginResponse> GamePlatformLoginBySM01Async(
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

        return response.StatusCode == HttpStatusCode.NoContent
            ? throw new Exception($"取得{GameProviderType.SM01}遊戲網址失敗.")
            : JsonSerializer.Deserialize<SM01PlatformLoginResponse>(
                await response
                .Content
                .ReadAsStringAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false),
                _SerializerOptions)
                ?? throw new Exception($"取得{GameProviderType.SM01}遊戲網址失敗.");
    }

    public async ValueTask<Uri> GameUrlAsync(
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

        return response.StatusCode == HttpStatusCode.NoContent
            ? throw new Exception($"取得{gameProviderCode}遊戲網址失敗.")
            : JsonSerializer.Deserialize<GameUrlResponse>(
                await response
                .Content
                .ReadAsStringAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false),
                _SerializerOptions)?.Url
                ?? throw new Exception($"取得{gameProviderCode}遊戲網址失敗.");
    }

    public async ValueTask<Uri> GameUrlByAsiaGamingAsync(
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

        return response.StatusCode == HttpStatusCode.NoContent
            ? throw new Exception($"取得{GameProviderType.AsiaGaming}遊戲網址失敗.")
            : JsonSerializer.Deserialize<GameUrlResponse>(
                await response
                .Content
                .ReadAsStringAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false),
                _SerializerOptions)?.Url
                ?? throw new Exception($"取得{GameProviderType.AsiaGaming}遊戲網址失敗.");
    }

    public async ValueTask<Uri> GameUrlByBetConstructAsync(
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

        return response.StatusCode == HttpStatusCode.NoContent
            ? throw new Exception($"取得{GameProviderType.BetConstruct}遊戲網址失敗.")
            : JsonSerializer.Deserialize<GameUrlResponse>(
                await response
                .Content
                .ReadAsStringAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false),
                _SerializerOptions)?.Url
                ?? throw new Exception($"取得{GameProviderType.BetConstruct}遊戲網址失敗.");
    }

    public async ValueTask KickPlayerAsync(string gameProviderCode, string playerCode, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient
          .DeleteAsync(
              requestUri: string.Format(_KickPlayerPath, gameProviderCode, playerCode),
              cancellationToken: cancellationToken).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async ValueTask<PlayerBalanceResponse> PlayerBalanceAsync(string gameProviderCode, string playerCode, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient
            .GetAsync(
                requestUri: string.Format(_PlayerBalancePath, gameProviderCode, playerCode),
                cancellationToken: cancellationToken).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        return response.StatusCode == HttpStatusCode.NoContent
            ? throw new Exception($"取得{gameProviderCode}餘額失敗.")
            : JsonSerializer.Deserialize<PlayerBalanceResponse>(
                await response
                .Content
                .ReadAsStringAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false),
                _SerializerOptions)!;
    }

    public async ValueTask<bool> PlayerFundCheckingAsync(string gameProviderCode, string brandCode, string playerCode, string currencyCode, string orderId, CancellationToken cancellationToken = default)
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

        return response.StatusCode == HttpStatusCode.NoContent
            ? throw new Exception($"取得{gameProviderCode}交易檢查狀態失敗.({orderId})")
            : JsonSerializer.Deserialize<PlayerFundCheckingResponse>(
                await response
                .Content
                .ReadAsStringAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false),
                _SerializerOptions)!.OrderStatus;
    }

    public async ValueTask<bool> PlayerFundCheckingByVRAsync(string brandCode, string playerCode, string currencyCode, string orderId, DateTimeOffset startTime, DateTimeOffset endTime, CancellationToken cancellationToken = default)
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
               requestUri: string.Format(_PlayerFundChecking, GameProviderType.VRGaming),
               content: requestContent,
               cancellationToken: cancellationToken).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        return response.StatusCode == HttpStatusCode.NoContent
            ? throw new Exception($"取得{GameProviderType.VRGaming}交易檢查狀態失敗.({orderId})")
            : JsonSerializer.Deserialize<PlayerFundCheckingResponse>(
                await response
                .Content
                .ReadAsStringAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false),
                _SerializerOptions)!.OrderStatus;
    }

    public async ValueTask<string> PlayerFundInAsync(string gameProviderCode, string brandCode, string playerCode, string currencyCode, string orderId, decimal amount, CancellationToken cancellationToken = default)
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

        return response.StatusCode == HttpStatusCode.NoContent
            ? throw new Exception($"玩家{gameProviderCode}轉入主錢包失敗.({orderId})")
            : JsonSerializer.Deserialize<PlayerFundResponse>(
                await response
                .Content
                .ReadAsStringAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false),
                _SerializerOptions)!.OrderId;
    }

    public async ValueTask<string> PlayerFundOutAsync(string gameProviderCode, string brandCode, string playerCode, string currencyCode, string orderId, decimal amount, CancellationToken cancellationToken = default)
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

        return response.StatusCode == HttpStatusCode.NoContent
            ? throw new Exception($"玩家主錢包轉入{gameProviderCode}失敗.({orderId})")
            : JsonSerializer.Deserialize<PlayerFundResponse>(
                await response
                .Content
                .ReadAsStringAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false),
                _SerializerOptions)!.OrderId;
    }

    public async ValueTask<RoundDetailResult> RoundDetailAsync(string gameProviderCode, Guid roundId, string language, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient
           .GetAsync(
               requestUri: string.Format(_RoundDetail, gameProviderCode, roundId, language),
               cancellationToken: cancellationToken).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        return response.StatusCode == HttpStatusCode.NoContent
            ? throw new Exception($"取得訂單明細{gameProviderCode}失敗.({roundId:N})")
            : JsonSerializer.Deserialize<RoundDetailResult>(
                await response
                .Content
                .ReadAsStringAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false),
                _SerializerOptions)!;
    }
}
