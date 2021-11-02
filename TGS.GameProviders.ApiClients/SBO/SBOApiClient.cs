using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.SBO.Models;
using TGS.GameProviders.Domains.Extensions;

namespace TGS.GameProviders.ApiClients.SBO
{
    public class SBOApiClient : ISBOApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<SBOApiClient> _logger;
        private readonly ITgsLog _tgsLog;

        public SBOApiClient(
            HttpClient client,
            ILogger<SBOApiClient> logger,
            ITgsLog tgsLog)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
        }

        public ValueTask<SBOGetBetHistoryResponse> GetBetHistoryAsync(Uri apiUrl, SBOGetBetHistoryRequest request)
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString
            };

            return PostAsync<SBOGetBetHistoryRequest, SBOGetBetHistoryResponse>(
                apiUrl,
                "/web-root/restricted/report/v2/get-bet-list-by-modify-date.aspx",
                request,
                "BetHistory",
                jsonSerializerOptions);
        }

        public ValueTask<SBOGetRoundHistoryResponse> GetRoundHistoryAsync(Uri apiUrl, SBOGetRoundHistoryRequest request)
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString
            };

            return PostAsync<SBOGetRoundHistoryRequest, SBOGetRoundHistoryResponse>(
                apiUrl,
                "/web-root/restricted/report/get-bet-list-by-refnos.aspx",
                request,
                "RoundHistory",
                jsonSerializerOptions);
        }

        public ValueTask<SBOLoginResponse> LoginAsync(Uri apiUrl, SBOLoginRequest request)
            => PostAsync<SBOLoginRequest, SBOLoginResponse>(
                apiUrl,
                "/web-root/restricted/player/login.aspx",
                request,
                "Login");

        public ValueTask<SBOLogoutResponse> LogoutAsync(Uri apiUrl, SBOLogoutRequest request)
            => PostAsync<SBOLogoutRequest, SBOLogoutResponse>(
                apiUrl,
                "/web-root/restricted/player/logout.aspx",
                request,
                "Logout");

        public ValueTask<SBORegisterPlayerResponse> RegisterPlayerAsync(Uri apiUrl, SBORegisterPlayerRequest request)
            => PostAsync<SBORegisterPlayerRequest, SBORegisterPlayerResponse>(
                apiUrl,
                "/web-root/restricted/player/register-player.aspx",
                request,
                "RegisterPlayer");

        public ValueTask<SBOUpdatePlayerBetLimitsResponse> UpdatePlayerBetLimitsAsync(Uri apiUrl,
            SBOUpdatePlayerBetLimitsRequest request)
            => PostAsync<SBOUpdatePlayerBetLimitsRequest, SBOUpdatePlayerBetLimitsResponse>(
                apiUrl,
                "/web-root/restricted/player/update-player-bet-setting-by-sportid-and-markettype.aspx",
                request, "UpdatePlayerBetLimit");

        private static string GetUriString(Uri apiUrl, string path)
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

        private async ValueTask<TResponse> PostAsync<TRequest, TResponse>(
                    Uri apiUrl,
            string path,
            TRequest request,
            string action,
            JsonSerializerOptions jsonSerializerOptions = null)
        {
            var logPrefix = $"{nameof(SBOApiClient).ToLower()}.{action}";
            var uriString = GetUriString(apiUrl, path);

            var sw = Stopwatch.StartNew();

            var data = JsonSerializer.Serialize(request);
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var httpResponseMessage = await _client.PostAsync(uriString, content).ConfigureAwait(false);

            sw.Stop();

            var responseMessage = default(string);

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
                RequestContent = data,
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
                return JsonSerializer.Deserialize<TResponse>(responseMessage, jsonSerializerOptions);
            }

            _logger.LogError(JsonSerializer.Serialize(apiLogEvent));

            return default;
        }
    }
}
