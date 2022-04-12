using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.BigGaming;
using Tgs.GameProviders.Core.BigGaming.Models;
using Tgs.GameProviders.Core.BigGaming.Models.Commons;
using Tgs.GameProviders.Core.Extensions;
using Tgs.Logging;

namespace Tgs.GameProviders.ApiClients.BigGaming
{
    internal class BigGamingApiClient : IBigGamingApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<BigGamingApiClient> _logger;
        private readonly ITgsLog _tgsLog;

        public BigGamingApiClient(
            HttpClient client,
            ILogger<BigGamingApiClient> logger,
            ITgsLog tgsLog)

        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
        }

        public async ValueTask<BigGamingResponse> CreateAgentAsync(Uri apiurl, string secretCode, string secretKey, BigGamingCreateAgentRequest request)
        {
            request.Sign = GetHashMD5($"{request.Random }{secretCode}{request.LoginId}{secretKey}");
            var prefix = $"CreateAgent";
            return await GetAPIAsync(apiurl, request, "open.agent.create", prefix).ConfigureAwait(false);
        }

        public async ValueTask<BigGamingResponse> CreatePlayerAsync(Uri apiurl, string secretCode, string agentLoginIdPassword, BigGamingCreatePlayerRequest request)
        {
            request.Digest = GetHashMD5($"{request.Random }{secretCode}{GetSecretCode(agentLoginIdPassword)}");
            var prefix = $"CreatePlayer";
            return await GetAPIAsync(apiurl, request, "open.user.create", prefix).ConfigureAwait(false);
        }

        public async ValueTask<BigGamingResponse> GetGameRecordAsync(Uri apiurl, string secretCode, string secretKey, BigGamingGameRecordFishingRequest request)
        {
            request.Sign = GetHashMD5($"{request.Random }{secretCode}{secretKey}");
            var prefix = $"GetGameRecordFishing";
            return await GetAPIAsync(apiurl, request, "open.order.bg.query", prefix).ConfigureAwait(false);
        }

        public async ValueTask<BigGamingResponse> GetGameRecordFishingUrlAsync(Uri apiurl, string secretCode, string secretKey, BigGamingGameRecordFishingUrlRequest request)
        {
            request.Sign = GetHashMD5($"{request.Random }{secretCode}{request.ExternalBetId}{secretKey}");
            var prefix = $"GetGameRecordFishingUrl";
            return await GetAPIAsync(apiurl, request, "open.sn.bg.order.detail.url.get", prefix).ConfigureAwait(false);
        }

        public async ValueTask<BigGamingResponse> GetGameUrlAsync(Uri apiurl, string secretCode, string agentLoginIdPassword, BigGamingGetGameUrlLiveDealerRequest request)
        {
            request.Digest = GetHashMD5($"{request.Random }{secretCode}{request.LoginId}{GetSecretCode(agentLoginIdPassword)}");
            var prefix = $"GetGameUrlLiveDealer";
            return await GetAPIAsync(apiurl, request, "open.video.game.url", prefix).ConfigureAwait(false);
        }

        public async ValueTask<BigGamingResponse> GetGameUrlAsync(Uri apiurl, string secretCode, string secretKey, BigGamingGetGameUrlFishingRequest request)
        {
            request.Sign = GetHashMD5($"{request.Random }{secretCode}{secretKey}");
            var prefix = $"GetGameUrlFish";
            return await GetAPIAsync(apiurl, request, "open.game.bg.url", prefix).ConfigureAwait(false);
        }

        public async ValueTask<BigGamingResponse> GetTrialGameUrlAsync(Uri apiurl, string secretCode, string secretKey, BigGamingGetTrialGameUrlRequest request)
        {
            request.Sign = GetHashMD5($"{request.Random }{secretCode}{secretKey}");
            var prefix = $"GetTrialGameUrl";
            return await GetAPIAsync(apiurl, request, "open.video.trial.game.url", prefix).ConfigureAwait(false);
        }

        public async ValueTask<BigGamingResponse> GetVideoOrderDetailAsync(Uri apiurl, string secretKey, BigGamingVideoOrderDetailRequest request)
        {
            request.Sign = GetHashMD5($"{request.Random }{request.Sn}{request.OrderId}{secretKey}");
            var prefix = $"VideoOrderDetail";
            return await GetAPIAsync(apiurl, request, "open.sn.video.order.detail.url.get", prefix).ConfigureAwait(false);
        }

        public async ValueTask<BigGamingResponse> GetVideoOrderQueryAsync(Uri apiurl, string secretKey, BigGamingVideoOrderQueryRequest request)
        {
            request.Sign = GetHashMD5($"{request.Random }{request.Sn}{secretKey}");
            var prefix = $"VideoOrderQuery";
            return await GetAPIAsync(apiurl, request, "open.order.query", prefix).ConfigureAwait(false);
        }

        public async ValueTask<BigGamingResponse> GetVideoRoundQueryAsync(Uri apiurl, string secretKey, BigGamingVideoRoundQueryRequest request)
        {
            request.Sign = GetHashMD5($"{request.Random }{request.Sn}{secretKey}");
            var prefix = $"VideoRoundQuery";
            return await GetAPIAsync(apiurl, request, "open.video.round.query", prefix).ConfigureAwait(false);
        }

        public async ValueTask<BigGamingResponse> InfoPlayerAsync(Uri apiurl, string agentLoginIdPassword, BigGamingInfoPlayerRequest request)
        {
            request.Digest = GetHashMD5($"{request.Random }{request.Sn}{request.LoginId}{GetSecretCode(agentLoginIdPassword)}");
            var prefix = $"InfoPlayer";
            return await GetAPIAsync(apiurl, request, "open.user.get", prefix).ConfigureAwait(false);
        }

        public async ValueTask<BigGamingResponse> KickPlayerAsync(Uri apiurl, string agentLoginIdPassword, BigGamingKickPlayerRequest request)
        {
            request.Digest = GetHashMD5($"{request.Random }{request.Sn}{request.LoginId}{GetSecretCode(agentLoginIdPassword)}");
            var prefix = $"KickPlayer";
            return await GetAPIAsync(apiurl, request, "open.user.logout", prefix).ConfigureAwait(false);
        }

        public async ValueTask<BigGamingResponse> KickPlayerGameAsync(Uri apiurl, string agentLoginIdPassword, BigGamingKickPlayerGameRequest request)
        {
            request.Digest = GetHashMD5($"{request.Random }{request.Sn}{request.LoginId}{GetSecretCode(agentLoginIdPassword)}");
            var prefix = $"KickPlayerGame";
            return await GetAPIAsync(apiurl, request, "open.user.bg.logout", prefix).ConfigureAwait(false);
        }

        private static string GetHashMD5(string data)
        {
            using var md5 = MD5.Create();
            var dataByte = Encoding.UTF8.GetBytes(data);
            var hash = md5.ComputeHash(dataByte);
            return hash.ToHexString().ToUpper();
        }

        private static string GetSecretCode(string agentLoginIdPassword)
        {
            using var sHA1 = SHA1.Create();
            var dataByte = Encoding.UTF8.GetBytes(agentLoginIdPassword);
            var hash = sHA1.ComputeHash(dataByte);
            return hash.ToBase64String();
        }

        private async ValueTask<BigGamingResponse> GetAPIAsync<TSource>(Uri apiUrl, TSource source, string method, string logPrefix)
        {
            logPrefix = $"{nameof(BigGamingApiClient).ToLower() }.{logPrefix}";
            var data = new BigGamingRequest()
            {
                Id = GuidExtensions.NewSequentialGuid().ToString(),
                Method = method,
                Params = JsonDocument.Parse(JsonSerializer.Serialize(source)),
                Jsonrpc = "2.0"
            };

            var sw = Stopwatch.StartNew();

            HttpContent content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

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
                RequestContent = JsonSerializer.Serialize(data).NoLongerThan(4096),
                ResponseHeaders = httpResponseMessage.Headers.ToString(),
                ResponseContent = responseMessage.NoLongerThan(4096),
                ResponseCode = (short)httpResponseMessage.StatusCode,
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                RecordedOn = DateTimeOffset.UtcNow
            };
            _tgsLog.LogApi(apiLogEvent);
            _logger.LogInformation("{JsonSerializer.Serialize(apiLogEvent)}", JsonSerializer.Serialize(apiLogEvent));

            return !string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK
                ? JsonSerializer.Deserialize<BigGamingResponse>(responseMessage)
                : default;
        }
    }
}
