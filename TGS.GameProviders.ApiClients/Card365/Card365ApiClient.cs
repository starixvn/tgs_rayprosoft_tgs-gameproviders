using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Logging;
using TGS.GameProviders.Core.Card365;
using TGS.GameProviders.Core.Card365.Model;
using TGS.GameProviders.Core.Extensions;

namespace TGS.GameProviders.ApiClients.Card365;

internal class Card365ApiClient : ICard365ApiClient
{
    private readonly HttpClient _client;
    private readonly ILogger<Card365ApiClient> _logger;
    private readonly ITgsLog _tgsLog;

    public Card365ApiClient(
        ILogger<Card365ApiClient> logger,
        HttpClient client,
        ITgsLog tgsLog)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
    }

    public ValueTask<Card365GetKeyResponse> GetAccessKeyAsync(
        Uri apiUrl,
        string website,
        Card365GetKeyRequest request)
    {
        var value = JsonSerializer.Serialize(request);
        var parameters = JsonSerializer.Deserialize<Dictionary<string, string>>(value);

        return PostAsync<Card365GetKeyResponse>(
            apiUrl,
            $"/{website}/getKey",
            parameters,
            "GetKey");
    }

    public ValueTask<Card365LogoutResponse> LogoutAsync(
        Uri apiUrl,
        string website,
        Card365LogoutRequest request)
    {
        var value = JsonSerializer.Serialize(request);
        var parameters = JsonSerializer.Deserialize<Dictionary<string, string>>(value);

        return PostAsync<Card365LogoutResponse>(
            apiUrl,
            $"/{website}/logout",
            parameters,
            "Logout");
    }

    private async ValueTask<TResponse> PostAsync<TResponse>(
        Uri apiUrl,
        string path,
        IEnumerable<KeyValuePair<string, string>> parameters,
        string action)
    {
        var logPrefix = $"{nameof(Card365ApiClient).ToLower()}.{action}";
        var uriString = $"{apiUrl.AbsoluteUri}{path}";

        var sw = Stopwatch.StartNew();

        var content = new FormUrlEncodedContent(parameters);
        var httpResponseMessage = await _client.PostAsync(uriString, content).ConfigureAwait(false);

        sw.Stop();

        var responseMessage = default(string);

        var requestMessage = await content.ReadAsStringAsync().ConfigureAwait(false);

        var stream = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
        using (var sr = new StreamReader(stream))
        {
            responseMessage = await sr.ReadToEndAsync().ConfigureAwait(false);
        }

        var apiLogEvent = new ApiLogEvent
        {
            Ip = string.Empty,
            Path = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
            Method = httpResponseMessage.RequestMessage?.Method.ToString().NoLongerThan(8) ?? string.Empty,
            Prefix = logPrefix,
            ServerName = Environment.MachineName.NoLongerThan(128),
            RequestHeaders =
                httpResponseMessage.RequestMessage?.Headers +
                httpResponseMessage.RequestMessage?.Content?.Headers?.ToString(),
            RequestContent = requestMessage,
            ResponseHeaders = httpResponseMessage.Headers.ToString(),
            ResponseContent = responseMessage,
            ResponseCode = (short)httpResponseMessage.StatusCode,
            ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
            RecordedOn = DateTimeOffset.UtcNow
        };

        _tgsLog.LogApi(apiLogEvent);

        if (!string.IsNullOrEmpty(responseMessage) &&
            httpResponseMessage.StatusCode == HttpStatusCode.OK)
        {
            return JsonSerializer.Deserialize<TResponse>(responseMessage);
        }

        _logger.LogError(JsonSerializer.Serialize(apiLogEvent));

        return default;
    }
}
