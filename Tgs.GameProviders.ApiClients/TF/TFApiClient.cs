using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Core.Infrastructures.Security;
using Tgs.GameProviders.ApiClients.TF.Models.Commons;
using Tgs.GameProviders.Core.Extensions;
using Tgs.GameProviders.Core.TFGaming;
using Tgs.Logging;

namespace Tgs.GameProviders.ApiClients.TF
{
    internal class TFApiClient : ITFApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<TFApiClient> _logger;
        private readonly ITgsLog _tgsLog;
        private readonly ITokenProvider _tokenProvider;

        public TFApiClient(
            HttpClient client,
            ILogger<TFApiClient> logger,
            ITgsLog tgsLog,
            ITokenProvider tokenProvider)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        }

        public async ValueTask<Uri> GetGameUrlAsync(Uri apiUrl, string publicToken, string playerCode)
        {
            var sw = Stopwatch.StartNew();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{apiUrl.OriginalString}/api/v4/launch/url/?auth={publicToken}"),
                Content = new StringContent("", Encoding.UTF8, MediaTypeNames.Application.Json)
            };

            var httpResponseMessage = await _client
                .SendAsync(request)
                .ConfigureAwait(false);

            sw.Stop();

            var responseMessage = await httpResponseMessage.Content
               .ReadAsStringAsync()
               .ConfigureAwait(false);

            var apiLogEvent = new ApiLogEvent
            {
                Path = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
                Method = httpResponseMessage.RequestMessage?.Method.ToString().NoLongerThan(8) ?? string.Empty,
                Prefix = TFAction.GetGameUrl.ToString(),
                ServerName = Environment.MachineName.NoLongerThan(128),
                RequestHeaders = httpResponseMessage.RequestMessage?.Headers + httpResponseMessage.RequestMessage?.Content?.Headers?.ToString(),
                RequestContent = JsonSerializer.Serialize(request).NoLongerThan(4096),
                ResponseHeaders = httpResponseMessage.Headers.ToString(),
                ResponseContent = responseMessage.NoLongerThan(4096),
                ResponseCode = (short)httpResponseMessage.StatusCode,
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                RecordedOn = DateTimeOffset.UtcNow
            };

            _tgsLog.LogApi(apiLogEvent);
            _logger.LogInformation(JsonSerializer.Serialize(apiLogEvent));

            _ = httpResponseMessage.EnsureSuccessStatusCode();

            var result = JsonSerializer.Deserialize<LaunchResponse>(responseMessage);

            var launchUrl = result.LaunchUrl;

            var token = await _tokenProvider
               .EncryptAsync(playerCode)
               .ConfigureAwait(false);

            return new Uri($"{launchUrl}?auth={publicToken}&token={token}");
        }

        public async ValueTask KickAsync(Uri apiUrl, string privateToken, string partnerId, string playerCode)
        {
            var sw = Stopwatch.StartNew();

            var request = new TFApiKickRequest
            {
                PartnerId = partnerId,
                PlayerCode = playerCode
            };

            var content = JsonSerializer.Serialize(request);
            var stringContent = new StringContent(content, Encoding.UTF8, MediaTypeNames.Application.Json);

            _client.DefaultRequestHeaders.Add("Authorization", $"Token {privateToken}");

            var httpResponseMessage = await _client
                .PostAsync($"{apiUrl.OriginalString}/api/v2/partner-account-logout/", stringContent)
                .ConfigureAwait(false);

            sw.Stop();

            var responseMessage = await httpResponseMessage.Content
               .ReadAsStringAsync()
               .ConfigureAwait(false);

            var apiLogEvent = new ApiLogEvent
            {
                Path = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
                Method = httpResponseMessage.RequestMessage?.Method.ToString().NoLongerThan(8) ?? string.Empty,
                Prefix = TFAction.Kick.ToString(),
                ServerName = Environment.MachineName.NoLongerThan(128),
                RequestHeaders = httpResponseMessage.RequestMessage?.Headers + httpResponseMessage.RequestMessage?.Content?.Headers?.ToString(),
                RequestContent = JsonSerializer.Serialize(request).NoLongerThan(4096),
                ResponseHeaders = httpResponseMessage.Headers.ToString(),
                ResponseContent = responseMessage.NoLongerThan(4096),
                ResponseCode = (short)httpResponseMessage.StatusCode,
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                RecordedOn = DateTimeOffset.UtcNow
            };

            _tgsLog.LogApi(apiLogEvent);
            _logger.LogInformation(JsonSerializer.Serialize(apiLogEvent));
        }
    }
}
