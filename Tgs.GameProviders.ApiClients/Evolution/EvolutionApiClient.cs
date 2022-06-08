using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.Evolution;
using Tgs.GameProviders.Core.Evolution.Models;
using Tgs.GameProviders.Core.Extensions;
using Tgs.Logging;

namespace Tgs.GameProviders.ApiClients.Evolution
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
            logPrefix = $"{nameof(EvolutionApiClient).ToLower()}.{logPrefix}";

            var sw = Stopwatch.StartNew();

            _client.DefaultRequestHeaders.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{casinoID}:{gameHistoryApiToken}"))}");

            var httpResponseMessage = await _client.GetAsync(apiUrl);

            sw.Stop();

            var responseMessage = default(string);
            using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result))
            {
                responseMessage = await sr.ReadToEndAsync() ?? string.Empty;
            }

            var apiLogEvent = new ApiLogEvent
            {
                Ip = string.Empty,
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
            _tgsLog.LogApi(apiLogEvent);
            _logger.LogInformation("{JsonSerializer.Serialize(apiLogEvent)}", JsonSerializer.Serialize(apiLogEvent));

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                return JsonSerializer.Deserialize<TResult>(responseMessage);
            }

            return default;
        }

        private async ValueTask<TResult> PostAsync<TSource, TResult>(Uri apiUrl, TSource source, string logPrefix)
        {
            logPrefix = $"{nameof(EvolutionApiClient).ToLower()}.{logPrefix}";

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
                Ip = string.Empty,
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
            _tgsLog.LogApi(apiLogEvent);
            _logger.LogInformation("{JsonSerializer.Serialize(apiLogEvent)}", JsonSerializer.Serialize(apiLogEvent));

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                return JsonSerializer.Deserialize<TResult>(responseMessage);
            }

            return default;
        }
    }
}
