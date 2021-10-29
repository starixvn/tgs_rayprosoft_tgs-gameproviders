using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.KY.Models;
using TGS.GameProviders.ApiClients.KY.Models.Commons;
using TGS.GameProviders.Domains.Extensions;

namespace TGS.GameProviders.ApiClients.KY
{
    internal class KYApiClient : IKYApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<KYApiClient> _logger;
        private readonly ITgsLog _tgsLog;
        private readonly IKYUtility _utility;

        public KYApiClient(
            HttpClient client,
            ILogger<KYApiClient> logger,
            ITgsLog tgsLog,
            IKYUtility utility)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
        }

        public ValueTask<KYApiResponse<KYFnudResponse>> FundAsync(Uri apiUrl, string deskey, string md5key, string agentId, DateTimeOffset dateTimeOffset, KYAction action, KYFnudRequest request)
        {
            request.Action = _utility.GetAction(action) ?? -1;

            var timestamp = dateTimeOffset.ToUnixTimeMilliseconds();
            var dataPath = _utility.GetPath(request);
            var param = _utility.AesEncrypt(dataPath, deskey, true);
            var plainText = $"{agentId}{timestamp}{md5key}";
            var key = _utility.HashMD5(plainText);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<KYFnudResponse>(apiUrl, action, apiKYRequest, "Fund");
        }

        public ValueTask<KYApiResponse<KYFundCheckingResponse>> FundCheckingAsync(Uri apiUrl, string deskey, string md5key, string agentId, KYFundCheckingRequest request)
        {
            var action = KYAction.FundChecking;
            request.Action = _utility.GetAction(action) ?? -1;

            var timestamp = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeMilliseconds();
            var dataPath = _utility.GetPath(request);
            var param = _utility.AesEncrypt(dataPath, deskey, true);
            var plainText = $"{agentId}{timestamp}{md5key}";
            var key = _utility.HashMD5(plainText);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<KYFundCheckingResponse>(apiUrl, action, apiKYRequest, "FundChecking");
        }

        public ValueTask<KYApiResponse<KYGetBalanceResponse>> GetBalanceAsync(Uri apiUrl, string deskey, string md5key, string agentId, KYGetBalanceRequest request)
        {
            var action = KYAction.GetBalance;
            request.Action = _utility.GetAction(action) ?? -1;

            var timestamp = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeMilliseconds();
            var dataPath = _utility.GetPath(request);
            var param = _utility.AesEncrypt(dataPath, deskey, true);
            var plainText = $"{agentId}{timestamp}{md5key}";
            var key = _utility.HashMD5(plainText);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<KYGetBalanceResponse>(apiUrl, action, apiKYRequest, "Balance");
        }

        public ValueTask<KYApiResponse<KYGameRecordResponse>> GetGameRecordAsync(Uri apiRecordUrl, string deskey, string md5key, string agentId, DateTimeOffset dateTimeOffset, KYGameRecordRequest request)
        {
            var action = KYAction.GameRecord;
            request.Action = _utility.GetAction(action) ?? -1;

            var timestamp = dateTimeOffset.ToUnixTimeMilliseconds();
            var dataPath = _utility.GetPath(request);
            var param = _utility.AesEncrypt(dataPath, deskey, true);
            var plainText = $"{agentId}{timestamp}{md5key}";
            var key = _utility.HashMD5(plainText);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<KYGameRecordResponse>(apiRecordUrl, action, apiKYRequest, "GameRecord");
        }

        public ValueTask<KYApiResponse<KYGameUrlResponse>> GetGameUrlAsync(Uri apiUrl, string deskey, string md5key, string agentId, DateTimeOffset dateTimeOffset, KYGameUrlRequest request)
        {
            var action = KYAction.GetGameUrl;
            request.Action = _utility.GetAction(KYAction.GetGameUrl) ?? -1;

            var timestamp = dateTimeOffset.ToUnixTimeMilliseconds();
            var dataPath = _utility.GetPath(request);
            var param = _utility.AesEncrypt(dataPath, deskey, true);
            var plainText = $"{agentId}{timestamp}{md5key}";
            var key = _utility.HashMD5(plainText);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<KYGameUrlResponse>(apiUrl, action, apiKYRequest, "GameUrl");
        }

        public ValueTask<KYApiResponse<KYKickResponse>> KickAsync(Uri apiUrl, string deskey, string md5key, string agentId, KYKickRequest request)
        {
            var action = KYAction.Kick;
            request.Action = _utility.GetAction(action) ?? -1;

            var timestamp = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeMilliseconds();
            var dataPath = _utility.GetPath(request);
            var param = _utility.AesEncrypt(dataPath, deskey, true);
            var plainText = $"{agentId}{timestamp}{md5key}";
            var key = _utility.HashMD5(plainText);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<KYKickResponse>(apiUrl, action, apiKYRequest, "Kick");
        }

        private async ValueTask<KYApiResponse<TData>> GetAPIAsync<TData>(Uri apiUrl, KYAction action, KYApiRequest request, string logPrefix)
        {
            logPrefix = $"{nameof(KYApiClient).ToLower() }.{logPrefix}";
            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(KYApiResponse<TData>);

            var paths = _utility.GetPath(request);
            var url = $"{apiUrl?.ToString()}?{paths}";

            var httpResponseMessage = await _client.GetAsync(url);
            sw.Stop();
            using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result))
            {
                responseMessage = await sr.ReadToEndAsync() ?? string.Empty;
            }

            _tgsLog.LogApi(new()
            {
                Ip = string.Empty,
                Path = httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri.NoLongerThan(512),
                Method = httpResponseMessage.RequestMessage.Method.ToString().NoLongerThan(8),
                Prefix = logPrefix,
                ServerName = Environment.MachineName.NoLongerThan(128),
                RequestHeaders = httpResponseMessage.RequestMessage.Headers + httpResponseMessage.RequestMessage.Content?.Headers?.ToString(),
                RequestContent = JsonSerializer.Serialize(request),
                ResponseHeaders = httpResponseMessage.RequestMessage.Headers.ToString(),
                ResponseContent = responseMessage,
                ResponseCode = (short)httpResponseMessage.StatusCode,
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                RecordedOn = DateTimeOffset.UtcNow
            });

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                response = JsonSerializer.Deserialize<KYApiResponse<TData>>(responseMessage);
            }
            else
            {
                _logger.LogError(JsonSerializer.Serialize(new
                {
                    Ip = string.Empty,
                    Path = httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri.NoLongerThan(512),
                    Method = httpResponseMessage.RequestMessage.Method.ToString().NoLongerThan(8),
                    Prefix = logPrefix,
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

            return response;
        }
    }
}
