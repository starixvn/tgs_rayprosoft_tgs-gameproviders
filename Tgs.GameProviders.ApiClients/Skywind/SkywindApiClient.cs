using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.Extensions;
using Tgs.GameProviders.Core.Skywind;
using Tgs.GameProviders.Core.Skywind.Models;

namespace Tgs.GameProviders.ApiClients.Skywind;

public class SkywindApiClient : ISkywindApiClient
{
    private readonly HttpClient _client;
    private readonly ILogger<SkywindApiClient> _logger;

    public SkywindApiClient(
        HttpClient client,
       ILogger<SkywindApiClient> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the user login asynchronous.
    /// AccessToken 有效時間:一個小时了
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <returns></returns>
    public async ValueTask<SkywindUserLoginResponse> GetAccessTokenAsync(Uri apiUrl, SkywindUserLoginRequest request)
    {
        apiUrl = new Uri($"{apiUrl}/login");
        return await PostAsync<SkywindUserLoginRequest, SkywindUserLoginResponse>(apiUrl, request, "UserLogin").ConfigureAwait(false);
    }

    public async ValueTask<SkywindGameUrlResponse> GetGameUrlAsync(Uri apiUrl, string getaccessToken, SkywindGameUrlRequest request)
    {
        apiUrl = new Uri($"{apiUrl}/players/{request.PlayerCode}/games/{request.GameCode}?ticket={request.Ticket}&language={request.Language}");
        return await GetAsync<SkywindGameUrlResponse>(apiUrl, getaccessToken, "UserLogin").ConfigureAwait(false);
    }

    public async ValueTask<SkywindGamHistoryRoundDetailsResponse> GetGamHistoryRoundDetailsAsync(Uri apiUrl, string getaccessToken, SkywindGamHistoryRoundDetailsRequest request)
    {
        apiUrl = new Uri($"{apiUrl}/history/game/{request.RoundId}/image?language={request.Language}");
        return await GetAsync<SkywindGamHistoryRoundDetailsResponse>(apiUrl, getaccessToken, "GamHistoryRoundDetails").ConfigureAwait(false);
    }

    private async ValueTask<TResult> GetAsync<TResult>(Uri apiUrl, string getaccessToken, string logPrefix)
    {
        logPrefix = $"{nameof(SkywindApiClient).ToLower()}.{logPrefix}";

        var sw = Stopwatch.StartNew();
        var responseMessage = default(string);

        _client.DefaultRequestHeaders.Add("X-ACCESS-TOKEN", getaccessToken);
        var httpResponseMessage = await _client.GetAsync(apiUrl);
        sw.Stop();
        using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result))
        {
            responseMessage = await sr.ReadToEndAsync() ?? string.Empty;
        }

        var apiLogEvent = new ApiLogEvent
        {
            Path = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
            Method = httpResponseMessage.RequestMessage?.Method.ToString().NoLongerThan(8) ?? string.Empty,
            Prefix = logPrefix,
            ServerName = Environment.MachineName.NoLongerThan(128),
            RequestHeaders = httpResponseMessage.RequestMessage?.Headers + httpResponseMessage.RequestMessage?.Content?.Headers?.ToString(),
            RequestContent = string.Empty,
            ResponseHeaders = httpResponseMessage.Headers.ToString(),
            ResponseContent = responseMessage.NoLongerThan(4096),
            ResponseCode = (short)httpResponseMessage.StatusCode,
            ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
            RecordedOn = DateTimeOffset.UtcNow
        };

        _logger.LogInformation("{apiLogEvent}", JsonSerializer.Serialize(apiLogEvent));

        if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
        {
            return JsonSerializer.Deserialize<TResult>(responseMessage);
        }

        return default;
    }

    private async ValueTask<TResult> PostAsync<TSource, TResult>(Uri apiUrl, TSource source, string logPrefix)
    {
        logPrefix = $"{nameof(SkywindApiClient).ToLower()}.{logPrefix}";

        var sw = Stopwatch.StartNew();

        HttpContent content = new StringContent(JsonSerializer.Serialize(source), Encoding.UTF8, "application/json");

        var httpResponseMessage = await _client.PostAsync(apiUrl, content);

        sw.Stop();

        var responseMessage = default(string);
        using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result))
        {
            responseMessage = await sr.ReadToEndAsync() ?? string.Empty;
        }

        var apiLogEvent = new ApiLogEvent
        {
            Path = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
            Method = httpResponseMessage.RequestMessage?.Method.ToString().NoLongerThan(8) ?? string.Empty,
            Prefix = logPrefix,
            ServerName = Environment.MachineName.NoLongerThan(128),
            RequestHeaders = httpResponseMessage.RequestMessage?.Headers + httpResponseMessage.RequestMessage?.Content?.Headers?.ToString(),
            RequestContent = (await content.ReadAsStringAsync().ConfigureAwait(false)).NoLongerThan(4096),
            ResponseHeaders = httpResponseMessage.Headers.ToString(),
            ResponseContent = responseMessage.NoLongerThan(4096),
            ResponseCode = (short)httpResponseMessage.StatusCode,
            ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
            RecordedOn = DateTimeOffset.UtcNow
        };

        _logger.LogInformation("{apiLogEvent}", JsonSerializer.Serialize(apiLogEvent));

        if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
        {
            return JsonSerializer.Deserialize<TResult>(responseMessage);
        }

        return default;
    }
}
