using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains;

namespace Tgs.GameProviders.Client;

internal class DreamGameClient : IDreamGameClient
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

    public DreamGameClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async ValueTask<GameRecordResponse?> GameRecordListAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient
           .GetAsync(
                requestUri: $"/api/{GameProviderType.DreamGame}/GameRecordList",
                cancellationToken: cancellationToken)
           .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<GameRecordResponse>(contentJson, _SerializerOptions);
    }

    public async ValueTask MarkGameRecordListAsync(IEnumerable<string> externalRoundIds, CancellationToken cancellationToken = default)
    {
        var source = externalRoundIds.ToArray();

        var requestContent = new StringContent(JsonSerializer.Serialize(source, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
           .PostAsync(
                requestUri: $"/api/{GameProviderType.DreamGame}/markgamerecordlist",
                content: requestContent,
                cancellationToken: cancellationToken)
           .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }
}
