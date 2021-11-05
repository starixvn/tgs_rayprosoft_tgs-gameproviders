using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Tgs.Logging;
using TGS.GameProviders.Core.AsiaGaming;
using TGS.GameProviders.Core.AsiaGaming.Models;
using TGS.GameProviders.Core.Extensions;

namespace TGS.GameProviders.ApiClients.AsiaGaming
{
    internal class AsiaGamingApiClient : IAsiaGamingApiClient
    {
        private readonly HttpClient _client;
        private readonly IAsiaGamingUtility _utility;
        private readonly ILogger<AsiaGamingApiClient> _logger;
        private readonly ITgsLog _tgsLog;

        public AsiaGamingApiClient(
            ILogger<AsiaGamingApiClient> logger,
            HttpClient client,
            IAsiaGamingUtility utility,
            ITgsLog tgsLog)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
        }

        public ValueTask<AsiaGamingCheckOrCreateAccountResponse> CheckOrCreateAccountAsync(
            Uri apiUrl,
            string md5Key,
            string desKey,
            AsiaGamingCheckOrCreateAccountRequest request)
        {
            var parameter = _utility.GetQueryString(request, "/\\\\/");

            var encodedParas = _utility.DesEncrypt(parameter, desKey);
            var key = _utility.Md5Hash($"{encodedParas}{md5Key}");

            return GetAsync<AsiaGamingCheckOrCreateAccountRequest, AsiaGamingCheckOrCreateAccountResponse>(
                apiUrl,
                "doBusiness.do",
                $"params={encodedParas}&key={key}",
                request,
                "CheckOrCreateAccount");
        }

        public ValueTask<AsiaGamingCreateGameSessionResponse> CreateGameSessionAsync(
            Uri apiUrl,
            AsiaGamingCreateGameSessionRequest request)
        {
            var queryString = _utility.GetQueryString(request, "&");

            return GetAsync<AsiaGamingCreateGameSessionRequest, AsiaGamingCreateGameSessionResponse>(
                apiUrl,
                "resource/player-tickets.ucs",
                queryString,
                request,
                "CreateGameSession");
        }

        private static string GetUriString(
            Uri apiUrl,
            string path,
            string queryString)
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

            if (!string.IsNullOrEmpty(queryString))
            {
                uriString += $"?{queryString}";
            }

            return uriString;
        }

        private async ValueTask<TResponse> GetAsync<TRequest, TResponse>(Uri apiUrl, string path, string queryString,
                            TRequest request, string action)
        {
            var logPrefix = $"{nameof(AsiaGamingApiClient).ToLower()}.{action}";
            var uriString = GetUriString(apiUrl, path, queryString);

            var sw = Stopwatch.StartNew();

            var httpResponseMessage = await _client.GetAsync(uriString).ConfigureAwait(false);

            sw.Stop();

            var requestContent = JsonSerializer.Serialize(request);
            var responseMessage = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

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
                RequestContent = requestContent,
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
                using var sr = new StringReader(responseMessage);
                var serializer = new XmlSerializer(typeof(TResponse));
                return (TResponse)serializer.Deserialize(sr);
            }

            _logger.LogError(JsonSerializer.Serialize(apiLogEvent));

            return default;
        }
    }
}