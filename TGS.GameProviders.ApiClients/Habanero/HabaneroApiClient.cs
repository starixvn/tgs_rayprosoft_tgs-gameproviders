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
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Core.Habanero;
using TGS.GameProviders.Core.Habanero.Models;

namespace TGS.GameProviders.ApiClients.Habanero
{
    internal class HabaneroApiClient : IHabaneroApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<HabaneroApiClient> _logger;
        private readonly ITgsLog _tgsLog;

        public HabaneroApiClient(
            HttpClient client,
            ILogger<HabaneroApiClient> logger,
            ITgsLog tgsLog)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
        }

        public ValueTask<HabaneroLogoutResponse> LogoutAsync(
            Uri apiUrl,
            HabaneroLogoutRequest request)
            => PostAsync<HabaneroLogoutRequest, HabaneroLogoutResponse>(
                apiUrl,
                "/jsonapi/logoutplayer",
                request,
                "Logout");

        private async ValueTask<TResponse> PostAsync<TRequest, TResponse>(
           Uri apiUrl,
           string path,
           TRequest request,
           string action,
           JsonSerializerOptions serializerResponseOptions = null)
        {
            var logPrefix = $"{nameof(HabaneroApiClient).ToLower()}.{action}";

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
                return JsonSerializer.Deserialize<TResponse>(responseMessage, serializerResponseOptions);
            }

            _logger.LogError(JsonSerializer.Serialize(apiLogEvent));

            return default;
        }
    }
}