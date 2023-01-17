using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Tgs.GameProviders.Core.Extensions;
using Tgs.GameProviders.Core.SexyGaming;
using Tgs.GameProviders.Core.SexyGaming.Models;
using Tgs.GameProviders.Domains;

namespace Tgs.GameProviders.ApiClients.SexyGaming;

internal class SexyGamingApiClient : ISexyGamingApiClient
{
    private readonly HttpClient _client;
    private readonly ILogger<SexyGamingApiClient> _logger;

    public SexyGamingApiClient(
        HttpClient client,
        ILogger<SexyGamingApiClient> logger
        )
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ValueTask<SexyGamingCreateMemberResponse> CreatePlayerAsync(Uri apiUrl, SexyGamingCreateMemberRequest request)
        => PostAsync<SexyGamingCreateMemberRequest, SexyGamingCreateMemberResponse>(apiUrl, "/wallet/createMember", request, $"CreatePlayerAsync");

    public async ValueTask<SexyGamingGameUrlResponse> GetGamingGameUrlAsync(Uri apiUrl, SexyGamingGameUrlRequest request)
    {
        var tryCount = 3;
        var seconds = 1;

        var retryPolicy = Policy.HandleResult<SexyGamingGameUrlResponse>(r
             => r.Status == ((int)SexyGamingGameApiError.UnableToProceedPleaseTryAgainLater).ToString("d4"))
         .WaitAndRetryAsync(tryCount,
             retryAttempt => TimeSpan.FromSeconds(seconds),
             (response, retryTime, context) =>
             {
                 var data = new
                 {
                     GameProviderCode = GameProviderType.SexyGaming,
                     Request = request,
                     Response = response,
                     Delay = retryTime,
                     CreationTime = DateTimeOffset.UtcNow
                 };
                 _logger.LogInformation("{data}", System.Text.Json.JsonSerializer.Serialize(data));
             });

        return await retryPolicy.ExecuteAsync(async ()
            => await PostAsync<SexyGamingGameUrlRequest, SexyGamingGameUrlResponse>(apiUrl, "/wallet/login", request, "GetGamingGameUrlAsync"));
    }

    public async ValueTask<SexyGamingGameUrlResponse> GetGamingLoginGameAsync(Uri apiUrl, SexyGamingLoginGameRequest request)
    {
        var tryCount = 3;
        var seconds = 1;

        var retryPolicy = Policy.HandleResult<SexyGamingGameUrlResponse>(r
             => r.Status == ((int)SexyGamingGameApiError.UnableToProceedPleaseTryAgainLater).ToString("d4"))
         .WaitAndRetryAsync(tryCount,
             retryAttempt => TimeSpan.FromSeconds(seconds),
             (response, retryTime, context) =>
             {
                 var data = new
                 {
                     GameProviderCode = GameProviderType.SexyGaming,
                     Request = request,
                     Response = response,
                     Delay = retryTime,
                     CreationTime = DateTimeOffset.UtcNow
                 };
                 _logger.LogInformation("{data}", System.Text.Json.JsonSerializer.Serialize(data));
             });

        return await retryPolicy.ExecuteAsync(async ()
            => await PostAsync<SexyGamingLoginGameRequest, SexyGamingGameUrlResponse>(apiUrl, "/wallet/doLoginAndLaunchGame", request, "GetGamingLoginGameAsync"));
    }

    public ValueTask<SexyGamingLogoutResponse> LogoutAsync(Uri apiUrl, SexyGamingLogoutRequest request)
        => PostAsync<SexyGamingLogoutRequest, SexyGamingLogoutResponse>(apiUrl, "/wallet/logout", request, "LogOut");

    private async ValueTask<TResult> PostAsync<TSource, TResult>(Uri apiUrl, string path, TSource source, string logPrefix)
    {
        logPrefix = $"{nameof(SexyGamingApiClient).ToLower()}.{logPrefix}";

        var sw = Stopwatch.StartNew();

        var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(System.Text.Json.JsonSerializer.Serialize(source));

        var url = new Uri($"{apiUrl.Scheme}://{apiUrl.Host}{path}");

        HttpContent content = new FormUrlEncodedContent(data);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
        {
            CharSet = "UTF-8"
        };

        var httpResponseMessage = await _client.PostAsync(url, content);

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

        _logger.LogInformation("{apiLogEvent}", System.Text.Json.JsonSerializer.Serialize(apiLogEvent));

        if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
        {
            return System.Text.Json.JsonSerializer.Deserialize<TResult>(responseMessage);
        }

        return default;
    }
}
