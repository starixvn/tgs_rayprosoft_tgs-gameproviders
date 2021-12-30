using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Core.Infrastructures.Security;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.TF.Models.Commons;
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Core.TFGaming;

namespace TGS.GameProviders.ApiClients.TF
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

            _tgsLog.LogApi(new()
            {
                Ip = string.Empty,
                Path = httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri.NoLongerThan(512),
                Method = httpResponseMessage.RequestMessage.Method.ToString().NoLongerThan(8),
                Prefix = TFAction.GetGameUrl.ToString(),
                ServerName = Environment.MachineName.NoLongerThan(128),
                RequestHeaders = httpResponseMessage.RequestMessage.Headers + httpResponseMessage.RequestMessage.Content?.Headers?.ToString(),
                RequestContent = JsonSerializer.Serialize(request),
                ResponseHeaders = httpResponseMessage.RequestMessage.Headers.ToString(),
                ResponseContent = responseMessage,
                ResponseCode = (short)httpResponseMessage.StatusCode,
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                RecordedOn = DateTimeOffset.UtcNow
            });

            if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(JsonSerializer.Serialize(new
                {
                    Ip = string.Empty,
                    Path = httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri.NoLongerThan(512),
                    Method = httpResponseMessage.RequestMessage.Method.ToString().NoLongerThan(8),
                    Prefix = TFAction.GetGameUrl.ToString(),
                    ServerName = Environment.MachineName.NoLongerThan(128),
                    RequestHeaders = httpResponseMessage.RequestMessage.Headers + httpResponseMessage.RequestMessage.Content?.Headers?.ToString(),
                    RequestContent = JsonSerializer.Serialize(request),
                    ResponseHeaders = httpResponseMessage.RequestMessage.Headers.ToString(),
                    ResponseContent = responseMessage,
                    ResponseCode = (short)httpResponseMessage.StatusCode,
                    ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                    RecordedOn = DateTimeOffset.UtcNow
                }));
            }

            httpResponseMessage.EnsureSuccessStatusCode();

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

            _tgsLog.LogApi(new()
            {
                Ip = string.Empty,
                Path = httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri.NoLongerThan(512),
                Method = httpResponseMessage.RequestMessage.Method.ToString().NoLongerThan(8),
                Prefix = TFAction.GetGameUrl.ToString(),
                ServerName = Environment.MachineName.NoLongerThan(128),
                RequestHeaders = httpResponseMessage.RequestMessage.Headers + httpResponseMessage.RequestMessage.Content?.Headers?.ToString(),
                RequestContent = content,
                ResponseHeaders = httpResponseMessage.RequestMessage.Headers.ToString(),
                ResponseContent = responseMessage,
                ResponseCode = (short)httpResponseMessage.StatusCode,
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                RecordedOn = DateTimeOffset.UtcNow
            });

            if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(JsonSerializer.Serialize(new
                {
                    Ip = string.Empty,
                    Path = httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri.NoLongerThan(512),
                    Method = httpResponseMessage.RequestMessage.Method.ToString().NoLongerThan(8),
                    Prefix = TFAction.GetGameUrl.ToString(),
                    ServerName = Environment.MachineName.NoLongerThan(128),
                    RequestHeaders = httpResponseMessage.RequestMessage.Headers + httpResponseMessage.RequestMessage.Content?.Headers?.ToString(),
                    RequestContent = content,
                    ResponseHeaders = httpResponseMessage.RequestMessage.Headers.ToString(),
                    ResponseContent = responseMessage,
                    ResponseCode = (short)httpResponseMessage.StatusCode,
                    ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                    RecordedOn = DateTimeOffset.UtcNow
                }));
            }
        }
    }
}