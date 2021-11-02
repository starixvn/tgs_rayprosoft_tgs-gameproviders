using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients.WMCasino.Models;
using TGS.GameProviders.Domains.Extensions;
using Tgs.Logging;

namespace TGS.GameProviders.ApiClients.WMCasino
{
    public class WMCasinoApiClient : IWMCasinoApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<WMCasinoApiClient> _logger;
        private readonly ITgsLog _tgsLog;

        public WMCasinoApiClient(
            HttpClient client,
            ILogger<WMCasinoApiClient> logger,
            ITgsLog tgsLog)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
        }

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

        public ValueTask<WMCasinoGetDateTimeReportResponse> GetDateTimeReportAsync(
            Uri apiUrl,
            WMCasinoGetDateTimeReportRequest request)
            => PostAsync<WMCasinoGetDateTimeReportRequest, WMCasinoGetDateTimeReportResponse>(
                apiUrl,
                request,
                "GetDateTimeReport");

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
}