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
using TGS.GameProviders.Core.Evolution;
using TGS.GameProviders.Core.Evolution.Models;
using TGS.GameProviders.Core.Extensions;

namespace TGS.GameProviders.ApiClients.Evolution
{
    internal class EvolutionApiClient : IEvolutionApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<EvolutionApiClient> _logger;
        private readonly ITgsLog _tgsLog;

        public EvolutionApiClient(
            HttpClient client,
            ILogger<EvolutionApiClient> logger,
            ITgsLog tgsLog
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
        }

        public ValueTask<EvolutionGameUrlResponse> GetGameUrlAsync(Uri ua2AuthenticationUrl, EvolutionGameUrlRequest request)
            => PostAsync<EvolutionGameUrlRequest, EvolutionGameUrlResponse>(ua2AuthenticationUrl, request, "GetGameUrl");

        public async ValueTask<EvolutionGamHistoryRoundResponse> GetGamHistoryRoundAsync(Uri apiUrl, string casinoID, string gameHistoryApiToken, EvolutionGamHistoryRoundRequest request)
        {
            var url = new Uri($"{apiUrl}api/gamehistory/v1/players/{request.PlayerCode}/games/{request.RoundId}");
            return await GetAsync<EvolutionGamHistoryRoundResponse>(url, casinoID, gameHistoryApiToken, "GetGamHistoryRound").ConfigureAwait(false);
        }

        private async ValueTask<TResult> GetAsync<TResult>(Uri apiUrl, string casinoID, string gameHistoryApiToken, string logPrefix)
        {
            logPrefix = $"{nameof(EvolutionApiClient).ToLower() }.{logPrefix}";

            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(TResult);
            _client.DefaultRequestHeaders.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{casinoID}:{gameHistoryApiToken}"))}");
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
                _logger.LogInformation(JsonSerializer.Serialize(new
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
            logPrefix = $"{nameof(EvolutionApiClient).ToLower() }.{logPrefix}";

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
