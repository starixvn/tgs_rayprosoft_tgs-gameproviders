using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
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
}
