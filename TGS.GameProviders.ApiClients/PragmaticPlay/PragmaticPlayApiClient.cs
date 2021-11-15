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
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Core.PragmaticPlay;
using TGS.GameProviders.Core.PragmaticPlay.Models;

namespace TGS.GameProviders.ApiClients.PragmaticPlay
{
    internal class PragmaticPlayApiClient : IPragmaticPlayApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<PragmaticPlayApiClient> _logger;
        private readonly ITgsLog _tgsLog;
        private readonly IPragmaticPlayUtility _utility;

        public PragmaticPlayApiClient(
            HttpClient client,
            ILogger<PragmaticPlayApiClient> logger,
            ITgsLog tgsLog,
            IPragmaticPlayUtility utility)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
        }

        public ValueTask<PragmaticPlayGameUrlResponse> GetGameUrlAsync(Uri apiUrl, string md5Key, PragmaticPlayGameUrlRequest request)
        {
            var value = JsonSerializer.Serialize(request);
            var parameters = JsonSerializer.Deserialize<Dictionary<string, string>>(value);

            var hash = _utility.Md5Hash(parameters, md5Key);
            parameters!["hash"] = hash;

            return PostAsync<PragmaticPlayGameUrlResponse>(
                apiUrl,
                "IntegrationService/v3/http/CasinoGameAPI/game/url",
                parameters,
                "GameUrl");
        }

        public ValueTask<PragmaticPlayOpenHistoryExtendedResponse> GetRoundHistoryAsync(Uri apiUrl, string md5Key, PragmaticPlayOpenHistoryExtendedRequest request)
        {
            var value = JsonSerializer.Serialize(request);
            var parameters = JsonSerializer.Deserialize<Dictionary<string, string>>(value);

            var hash = _utility.Md5Hash(parameters, md5Key);
            parameters!["hash"] = hash;

            return PostAsync<PragmaticPlayOpenHistoryExtendedResponse>(
                apiUrl,
                "IntegrationService/v3/http/HistoryAPI/OpenHistoryExtended/",
                parameters,
                "RoundHistory");
        }

        public ValueTask<PragmaticPlayLogoutResponse> LogoutAsync(Uri apiUrl, string md5Key, PragmaticPlayLogoutRequest request)
        {
            var value = JsonSerializer.Serialize(request);
            var parameters = JsonSerializer.Deserialize<Dictionary<string, string>>(value);

            var hash = _utility.Md5Hash(parameters, md5Key);
            parameters!["hash"] = hash;

            return PostAsync<PragmaticPlayLogoutResponse>(
                apiUrl,
                "IntegrationService/v3/http/CasinoGameAPI/game/session/terminate/",
                parameters,
                "Logout");
        }

        private static string GetUriString(
                    Uri apiUrl,
            string path)
        {
            const char slash = '/';

            char[] urlTrimChars =
            {
                slash,
                ' '
            };

            var uriString = apiUrl.AbsoluteUri.Trim(urlTrimChars);

            if (!string.IsNullOrWhiteSpace(path))
            {
                uriString += $"{slash}{path.TrimStart(urlTrimChars)}";
            }

            return uriString;
        }

        private async ValueTask<TResponse> PostAsync<TResponse>(Uri apiUrl, string path,
                    IEnumerable<KeyValuePair<string, string>> parameters, string action)
        {
            var logPrefix = $"{nameof(PragmaticPlayApiClient).ToLower()}.{action}";
            var uriString = GetUriString(apiUrl, path);

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
}