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
using Tgs.GameProviders.Core.Extensions;
using Tgs.GameProviders.Core.SBO;
using Tgs.GameProviders.Core.SBO.Models;
using Tgs.Logging;

namespace Tgs.GameProviders.ApiClients.SBO
{
    internal class SBOApiClient : ISBOApiClient
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

        public ValueTask<SBOCreateAgentResponse> CreateAgentAsync(
            Uri apiUrl,
            SBOCreateAgentRequest request)
            => PostAsync<SBOCreateAgentRequest, SBOCreateAgentResponse>(
                apiUrl,
                "/web-root/restricted/agent/register-agent.aspx",
                request,
                "CreateAgent");

        public ValueTask<SBOCreatePlayerResponse> CreatePlayerAsync(
            Uri apiUrl,
            SBOCreatePlayerRequest request)
            => PostAsync<SBOCreatePlayerRequest, SBOCreatePlayerResponse>(
                apiUrl,
                "/web-root/restricted/player/register-player.aspx",
                request,
                "CreatePlayer");

        public ValueTask<SBOGetBetHistoryResponse> GetBetHistoryAsync(
                    Uri apiUrl,
            SBOGetBetHistoryRequest request)
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString,
                Converters =
                {
                    new SBONumberToStringConverter()
                }
            };

            return PostAsync<SBOGetBetHistoryRequest, SBOGetBetHistoryResponse>(
                apiUrl,
                "/web-root/restricted/report/v2/get-bet-list-by-modify-date.aspx",
                request,
                "BetHistory",
                jsonSerializerOptions);
        }

        public ValueTask<SBOGetRoundHistoryResponse> GetRoundHistoryAsync(
            Uri apiUrl,
            SBOGetRoundHistoryRequest request)
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString,
                Converters =
                {
                    new SBONumberToStringConverter()
                }
            };

            return PostAsync<SBOGetRoundHistoryRequest, SBOGetRoundHistoryResponse>(
                apiUrl,
                "/web-root/restricted/report/get-bet-list-by-refnos.aspx",
                request,
                "RoundHistory",
                jsonSerializerOptions);
        }

        public ValueTask<SBOLoginResponse> LoginAsync(
            Uri apiUrl,
            SBOLoginRequest request)
            => PostAsync<SBOLoginRequest, SBOLoginResponse>(
                apiUrl,
                "/web-root/restricted/player/login.aspx",
                request,
                "Login");

        public ValueTask<SBOLogoutResponse> LogoutAsync(
            Uri apiUrl,
            SBOLogoutRequest request)
            => PostAsync<SBOLogoutRequest, SBOLogoutResponse>(
                apiUrl,
                "/web-root/restricted/player/logout.aspx",
                request,
                "Logout");

        public ValueTask<SBOUpdatePlayerBetLimitsResponse> UpdatePlayerBetLimitsAsync(
            Uri apiUrl,
            SBOUpdatePlayerBetLimitsRequest request)
            => PostAsync<SBOUpdatePlayerBetLimitsRequest, SBOUpdatePlayerBetLimitsResponse>(
                apiUrl,
                "/web-root/restricted/player/update-player-bet-setting-by-sportid-and-markettype.aspx",
                request,
                "UpdatePlayerBetLimit");

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
                Path = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
                Method = httpResponseMessage.RequestMessage?.Method.ToString().NoLongerThan(8) ?? string.Empty,
                Prefix = logPrefix,
                ServerName = Environment.MachineName.NoLongerThan(128),
                RequestHeaders = httpResponseMessage.RequestMessage?.Headers + httpResponseMessage.RequestMessage?.Content?.Headers?.ToString(),
                RequestContent = data.NoLongerThan(4096),
                ResponseHeaders = httpResponseMessage.Headers.ToString(),
                ResponseContent = responseMessage.NoLongerThan(4096),
                ResponseCode = (short)httpResponseMessage.StatusCode,
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                RecordedOn = DateTimeOffset.UtcNow
            };
            _tgsLog.LogApi(apiLogEvent);
            _logger.LogInformation("{JsonSerializer.Serialize(apiLogEvent)}", JsonSerializer.Serialize(apiLogEvent));

            if (!string.IsNullOrEmpty(responseMessage) &&
                httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                return JsonSerializer.Deserialize<TResponse>(responseMessage, jsonSerializerOptions);
            }

            return default;
        }
    }
}
