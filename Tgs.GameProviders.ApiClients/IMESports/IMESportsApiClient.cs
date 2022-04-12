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
using Tgs.GameProviders.Core.IMESports;
using Tgs.GameProviders.Core.IMESports.Models;
using Tgs.Logging;

namespace Tgs.GameProviders.ApiClients.IMESports
{
    internal class IMESportsApiClient : IIMESportsApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<IMESportsApiClient> _logger;
        private readonly ITgsLog _tgsLog;

        public IMESportsApiClient(
            ILogger<IMESportsApiClient> logger,
            HttpClient client,
            ITgsLog tgsLog)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
        }

        public ValueTask<IMESportsGetRoundHistoryResponse> GetRoundHistoryAsync(
            Uri apiUrl,
            IMESportsGetRoundHistoryRequest request)
        {
            var options = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                Converters =
                {
                    new JsonStringEnumConverter()
                }
            };

            return PostAsync<IMESportsGetRoundHistoryRequest, IMESportsGetRoundHistoryResponse>(
                apiUrl,
                "/Report/GetBetLog",
                request,
                "GetRoundHistory",
                options);
        }

        public ValueTask<IMESportsLaunchGameResponse> LaunchDesktopGameAsync(
            Uri apiUrl,
            IMESportsLaunchGameRequest request)
            => PostAsync<IMESportsLaunchGameRequest, IMESportsLaunchGameResponse>(
                apiUrl,
                "/Game/NewLaunchGame",
                request,
                "LaunchDesktopGame");

        public ValueTask<IMESportsLaunchGameResponse> LaunchMobileGameAsync(
            Uri apiUrl,
            IMESportsLaunchGameRequest request)
            => PostAsync<IMESportsLaunchGameRequest, IMESportsLaunchGameResponse>(
                    apiUrl,
                    "/Game/NewLaunchMobileGame",
                    request,
                    "LaunchMobileGame");

        public ValueTask<IMESportsRegisterPlayerResponse> RegisterPlayerAsync(
            Uri apiUrl,
            IMESportsRegisterPlayerRequest request)
                => PostAsync<IMESportsRegisterPlayerRequest, IMESportsRegisterPlayerResponse>(
                    apiUrl,
                    "/Player/Register",
                    request,
                    "RegisterPlayer");

        private async ValueTask<TResponse> PostAsync<TRequest, TResponse>(
            Uri apiUrl,
            string path,
            TRequest request,
            string action,
            JsonSerializerOptions serializerResponseOptions = null)
        {
            var logPrefix = $"{nameof(IMESportsApiClient).ToLower()}.{action}";

            var uriString = $"{apiUrl.AbsoluteUri}{path}";

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
                return JsonSerializer.Deserialize<TResponse>(responseMessage, serializerResponseOptions);
            }

            return default;
        }
    }
}
