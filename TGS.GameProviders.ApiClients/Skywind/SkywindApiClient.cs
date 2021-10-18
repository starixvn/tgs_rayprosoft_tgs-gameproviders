using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.Skywind.Models;
using TGS.GameProviders.Domains.Extensions;

namespace TGS.GameProviders.ApiClients.Skywind
{
    public class SkywindApiClient : ISkywindApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<SkywindApiClient> _logger;
        private readonly ITgsLog _tgsLog;

        public SkywindApiClient(
            HttpClient client,
           ILogger<SkywindApiClient> logger,
           ITgsLog tgsLog)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
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
            logPrefix = $"{nameof(SkywindApiClient).ToLower() }.{logPrefix}";

            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(TResult);
            _client.DefaultRequestHeaders.Add("X-ACCESS-TOKEN", getaccessToken);
            var httpResponseMessage = await _client.GetAsync(apiUrl);
            sw.Stop();
            using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result))
            {
                responseMessage = await sr.ReadToEndAsync() ?? string.Empty;
            }

            _tgsLog.LogApi(new()
            {
                Ip = string.Empty,
                Path = httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri.NoLongerThan(512),
                Method = httpResponseMessage.RequestMessage.Method.ToString().NoLongerThan(8),
                Prefix = logPrefix,
                ServerName = Environment.MachineName.NoLongerThan(128),
                RequestHeaders = httpResponseMessage.RequestMessage.Headers + httpResponseMessage.RequestMessage.Content?.Headers?.ToString(),
                RequestContent = string.Empty,
                ResponseHeaders = httpResponseMessage.RequestMessage.Headers.ToString(),
                ResponseContent = responseMessage,
                ResponseCode = (short)httpResponseMessage.StatusCode,
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                RecordedOn = DateTimeOffset.UtcNow
            });

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                response = JsonSerializer.Deserialize<TResult>(responseMessage);
            }
            else
            {
                _logger.LogError(JsonSerializer.Serialize(new
                {
                    Ip = string.Empty,
                    Path = httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri.NoLongerThan(512),
                    Method = httpResponseMessage.RequestMessage.Method.ToString().NoLongerThan(8),
                    Prefix = logPrefix,
                    ServerName = Environment.MachineName.NoLongerThan(128),
                    RequestHeaders = httpResponseMessage.RequestMessage.Headers + httpResponseMessage.RequestMessage.Content?.Headers?.ToString(),
                    RequestContent = string.Empty,
                    ResponseHeaders = httpResponseMessage.RequestMessage.Headers.ToString(),
                    ResponseContent = responseMessage,
                    ResponseCode = (short)httpResponseMessage.StatusCode,
                    ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                    RecordedOn = DateTimeOffset.UtcNow
                }));
            }

            return response;
        }

        private async ValueTask<TResult> PostAsync<TSource, TResult>(Uri apiUrl, TSource source, string logPrefix)
        {
            logPrefix = $"{nameof(SkywindApiClient).ToLower() }.{logPrefix}";

            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(TResult);
            HttpContent content = new StringContent(JsonSerializer.Serialize(source), Encoding.UTF8, "application/json");
            var httpResponseMessage = await _client.PostAsync(apiUrl, content);
            sw.Stop();
            using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result))
            {
                responseMessage = await sr.ReadToEndAsync() ?? string.Empty;
            }

            _tgsLog.LogApi(new()
            {
                Ip = string.Empty,
                Path = httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri.NoLongerThan(512),
                Method = httpResponseMessage.RequestMessage.Method.ToString().NoLongerThan(8),
                Prefix = logPrefix,
                ServerName = Environment.MachineName.NoLongerThan(128),
                RequestHeaders = httpResponseMessage.RequestMessage.Headers + httpResponseMessage.RequestMessage.Content?.Headers?.ToString(),
                RequestContent = JsonSerializer.Serialize(source),
                ResponseHeaders = httpResponseMessage.RequestMessage.Headers.ToString(),
                ResponseContent = responseMessage,
                ResponseCode = (short)httpResponseMessage.StatusCode,
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                RecordedOn = DateTimeOffset.UtcNow
            });

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                response = JsonSerializer.Deserialize<TResult>(responseMessage);
            }
            else
            {
                _logger.LogError(JsonSerializer.Serialize(new
                {
                    Ip = string.Empty,
                    Path = httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri.NoLongerThan(512),
                    Method = httpResponseMessage.RequestMessage.Method.ToString().NoLongerThan(8),
                    Prefix = logPrefix,
                    ServerName = Environment.MachineName.NoLongerThan(128),
                    RequestHeaders = httpResponseMessage.RequestMessage.Headers + httpResponseMessage.RequestMessage.Content?.Headers?.ToString(),
                    RequestContent = JsonSerializer.Serialize(source),
                    ResponseHeaders = httpResponseMessage.RequestMessage.Headers.ToString(),
                    ResponseContent = responseMessage,
                    ResponseCode = (short)httpResponseMessage.StatusCode,
                    ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                    RecordedOn = DateTimeOffset.UtcNow
                }));
            }

            return response;
        }
    }
}
