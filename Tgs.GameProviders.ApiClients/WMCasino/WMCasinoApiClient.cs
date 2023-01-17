using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.Extensions;
using Tgs.GameProviders.Core.WMCasino;
using Tgs.GameProviders.Core.WMCasino.Models;

namespace Tgs.GameProviders.ApiClients.WMCasino;

public class WMCasinoApiClient : IWMCasinoApiClient
{
    private readonly HttpClient _client;
    private readonly ILogger<WMCasinoApiClient> _logger;

    public WMCasinoApiClient(
        HttpClient client,
        ILogger<WMCasinoApiClient> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ValueTask<WMCasinoGetDateTimeReportResponse> GetDateTimeReportAsync(
        Uri apiUrl,
        WMCasinoGetDateTimeReportRequest request)
        => PostAsync<WMCasinoGetDateTimeReportRequest, WMCasinoGetDateTimeReportResponse>(
            apiUrl,
            request,
            "GetDateTimeReport");

    public ValueTask<WMCasinoLoginGameResponse> LoginGameAsync(
        Uri apiUrl,
        WMCasinoLoginGameRequest request)
        => PostAsync<WMCasinoLoginGameRequest, WMCasinoLoginGameResponse>(
            apiUrl,
            request,
            "LoginGame");

    public ValueTask<WMCasinoLogoutGameResponse> LogoutGameAsync(Uri apiUrl, WMCasinoLogoutGameRequest request)
        => PostAsync<WMCasinoLogoutGameRequest, WMCasinoLogoutGameResponse>(
            apiUrl,
            request,
            "LogoutGame");

    public ValueTask<WMCasinoMemberRegisterResponse> MemberRegisterAsync(
        Uri apiUrl,
        WMCasinoMemberRegisterRequest request)
        => PostAsync<WMCasinoMemberRegisterRequest, WMCasinoMemberRegisterResponse>(
            apiUrl,
            request,
            "MemberRegister");

    private async ValueTask<TResponse> PostAsync<TRequest, TResponse>(Uri apiUrl, TRequest request, string action)
    {
        var logPrefix = $"{nameof(WMCasinoApiClient).ToLower()}.{action}";

        var value = JsonSerializer.Serialize(request);
        var parameters = JsonSerializer.Deserialize<Dictionary<string, string>>(value);

        var sw = Stopwatch.StartNew();

        var content = new FormUrlEncodedContent(parameters!);
        var httpResponseMessage = await _client.PostAsync(apiUrl, content).ConfigureAwait(false);

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
            Path = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
            Method = httpResponseMessage.RequestMessage?.Method.ToString().NoLongerThan(8) ?? string.Empty,
            Prefix = logPrefix,
            ServerName = Environment.MachineName.NoLongerThan(128),
            RequestHeaders = httpResponseMessage.RequestMessage?.Headers + httpResponseMessage.RequestMessage?.Content?.Headers?.ToString(),
            RequestContent = requestMessage.NoLongerThan(4096),
            ResponseHeaders = httpResponseMessage.Headers.ToString(),
            ResponseContent = responseMessage.NoLongerThan(4096),
            ResponseCode = (short)httpResponseMessage.StatusCode,
            ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
            RecordedOn = DateTimeOffset.UtcNow
        };

        _logger.LogInformation("{apiLogEvent}", JsonSerializer.Serialize(apiLogEvent));

        if (!string.IsNullOrEmpty(responseMessage) &&
            httpResponseMessage.StatusCode == HttpStatusCode.OK)
        {
            return JsonSerializer.Deserialize<TResponse>(responseMessage);
        }

        return default;
    }
}
