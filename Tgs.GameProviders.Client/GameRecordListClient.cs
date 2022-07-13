using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains;

namespace Tgs.GameProviders.Client;

internal class GameRecordListClient : IGameRecordListClient
{
    private static readonly string _GameRecordListPath = "/api/{0}/gamerecordlist";

    private static readonly JsonSerializerOptions? _SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    private readonly HttpClient _httpClient;

    public GameRecordListClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async ValueTask<GameRecordResponse?> AviaGameRecordListAsync(
        string brandCode,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        int page,
        int rows,
        CancellationToken cancellationToken = default)
    {
        var source = new
        {
            BrandCode = brandCode,
            StartTime = startTime,
            EndTime = endTime,
            Page = page,
            Rows = rows
        };
        var requestContent = new StringContent(JsonSerializer.Serialize(source, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PostAsync(
                requestUri: $"/api/{GameProviderType.Avia}/GameRecordList",
                content: requestContent,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<GameRecordResponse>(contentJson, _SerializerOptions);
    }

    public async ValueTask<GameRecordResponse?> BigGamingFishingGameRecordListAsync(
        string currencyCode,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        int page,
        int rows,
        CancellationToken cancellationToken = default)
    {
        var source = new
        {
            CurrencyCode = currencyCode,
            StartTime = startTime,
            EndTime = endTime,
            Page = page,
            Rows = rows
        };
        var requestContent = new StringContent(JsonSerializer.Serialize(source, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PostAsync(
                requestUri: $"/api/{GameProviderType.BigGaming}/FishingGameRecordList",
                content: requestContent,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<GameRecordResponse>(contentJson, _SerializerOptions);
    }

    public async ValueTask<JokerGameRecordResponse?> JokerGameRecordListAsync(
        string brandCode,
        string agentId,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        string? nextId,
        CancellationToken cancellationToken = default)
    {
        var source = new
        {
            BrandCode = brandCode,
            AgentId = agentId,
            StartTime = startTime,
            EndTime = endTime,
            NextId = nextId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(source, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PostAsync(
                requestUri: string.Format(_GameRecordListPath, GameProviderType.Joker),
                content: requestContent,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<JokerGameRecordResponse>(contentJson, _SerializerOptions);
    }

    public async ValueTask<GameRecordResponse?> KYGameRecordListAsync(
        string brandCode,
        string currencyCode,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        CancellationToken cancellationToken = default)
    {
        var source = new
        {
            BrandCode = brandCode,
            CurrencyCode = currencyCode,
            StartTime = startTime,
            EndTime = endTime
        };
        var requestContent = new StringContent(JsonSerializer.Serialize(source, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PostAsync(
                requestUri: string.Format(_GameRecordListPath, GameProviderType.KY),
                content: requestContent,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<GameRecordResponse>(contentJson, _SerializerOptions);
    }

    public async ValueTask<GameRecordResponse?> LYGameRecordListAsync(
        string brandCode,
        string currencyCode,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        CancellationToken cancellationToken = default)
    {
        var source = new
        {
            BrandCode = brandCode,
            CurrencyCode = currencyCode,
            StartTime = startTime,
            EndTime = endTime
        };
        var requestContent = new StringContent(JsonSerializer.Serialize(source, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PostAsync(
                requestUri: string.Format(_GameRecordListPath, GameProviderType.LY),
                content: requestContent,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<GameRecordResponse>(contentJson, _SerializerOptions);
    }

    public async ValueTask<PragmaticPlayTransferGameRecordResponse?> PragmaticPlayTransferGameRecordListAsync(
        PragmaticPlayTransferGameDataType dataType,
        DateTimeOffset startTime,
        CancellationToken cancellationToken = default)
    {
        var source = new
        {
            DataType = dataType,
            StartTime = startTime
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(source, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PostAsync(
                requestUri: string.Format(_GameRecordListPath, GameProviderType.PragmaticPlayTransfer),
                content: requestContent,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<PragmaticPlayTransferGameRecordResponse>(contentJson, _SerializerOptions);
    }

    public async ValueTask<GameRecordResponse?> VRGameRecordListAsync(
        string brandCode,
        bool isUpdateTime,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        int page,
        int rows,
        CancellationToken cancellationToken = default)
    {
        var source = new
        {
            BrandCode = brandCode,
            IsUpdateTime = isUpdateTime,
            StartTime = startTime,
            EndTime = endTime,
            Page = page,
            Rows = rows
        };
        var requestContent = new StringContent(JsonSerializer.Serialize(source, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PostAsync(
                requestUri: string.Format(_GameRecordListPath, GameProviderType.VR),
                content: requestContent,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<GameRecordResponse>(contentJson, _SerializerOptions);
    }

    public async ValueTask<GameRecordResponse?> WGHorseGameRecordListAsync(
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        int page,
        int rows,
        CancellationToken cancellationToken = default)
    {
        var source = new
        {
            StartTime = startTime,
            EndTime = endTime,
            Page = page,
            Rows = rows
        };
        var requestContent = new StringContent(JsonSerializer.Serialize(source, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PostAsync(
                requestUri: string.Format(_GameRecordListPath, GameProviderType.WGHorse),
                content: requestContent,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<GameRecordResponse>(contentJson, _SerializerOptions);
    }

    public async ValueTask<GameRecordResponse?> WGSportsGameRecordListAsync(
        bool isSettlement,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        CancellationToken cancellationToken = default)
    {
        var source = new
        {
            IsSettlement = isSettlement,
            StartTime = startTime,
            EndTime = endTime
        };
        var requestContent = new StringContent(JsonSerializer.Serialize(source, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PostAsync(
                requestUri: string.Format(_GameRecordListPath, GameProviderType.WGSports),
                content: requestContent,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<GameRecordResponse>(contentJson, _SerializerOptions);
    }
}
