using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Logging;
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Core.Saba;
using TGS.GameProviders.Core.Saba.Models;

namespace TGS.GameProviders.ApiClients.Saba
{
    internal class SabaApiClient : ISabaApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<SabaApiClient> _logger;
        private readonly ITgsLog _tgsLog;

        public SabaApiClient(
            ILogger<SabaApiClient> logger,
            HttpClient client,
            ITgsLog tgsLog)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
        }

        public ValueTask<SabaCreateMemberResponse> CreateMemberAsync(Uri apiUrl, SabaCreateMemberRequest request)
        {
            var options = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString
            };

            var value = JsonSerializer.Serialize(request, options);
            var parameters = JsonSerializer.Deserialize<Dictionary<string, string>>(value);

            return PostAsync<SabaCreateMemberResponse>(
                apiUrl,
                "/api/CreateMember",
                parameters,
                "CreateMember");
        }

        public ValueTask<SabaGetRoundHistoryResponse> GetBetDetailByTransIdAsync(Uri apiUrl, SabaGetRoundHistoryRequest request)
        {
            var value = JsonSerializer.Serialize(request);
            var parameters = JsonSerializer.Deserialize<Dictionary<string, string>>(value);

            return PostAsync<SabaGetRoundHistoryResponse>(
                apiUrl,
                "/api/GetBetDetailByTransID",
                parameters,
                "GetRoundDetail");
        }

        public ValueTask<SabaGameUrlResponse> GetGameUrlAsync(Uri apiUrl, SabaGameUrlRequest request)
        {
            var value = JsonSerializer.Serialize(request);
            var parameters = JsonSerializer.Deserialize<Dictionary<string, string>>(value);

            return PostAsync<SabaGameUrlResponse>(
                apiUrl,
                "/api/GetSabaUrl",
                parameters,
                "GetSabaUrl");
        }

        public ValueTask<SabaKickUserResponse> KickUserAsync(Uri apiUrl, SabaKickUserRequest request)
        {
            var value = JsonSerializer.Serialize(request);
            var parameters = JsonSerializer.Deserialize<Dictionary<string, string>>(value);

            return PostAsync<SabaKickUserResponse>(
                apiUrl,
                "/api/KickUser",
                parameters,
                "KickUser");
        }

        private async ValueTask<TResponse> PostAsync<TResponse>(
            Uri apiUrl,
            string path,
            IEnumerable<KeyValuePair<string, string>> parameters,
            string action)
        {
            var logPrefix = $"{nameof(SabaApiClient).ToLower()}.{action}";

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
                _logger.LogInformation(responseMessage);

                return JsonSerializer.Deserialize<TResponse>(responseMessage);
            }

            _logger.LogError(JsonSerializer.Serialize(apiLogEvent));

            return default;
        }
    }
}
