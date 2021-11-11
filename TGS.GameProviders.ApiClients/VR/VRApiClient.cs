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
using TGS.GameProviders.Core.VR;
using TGS.GameProviders.Core.VR.Models;
using TGS.GameProviders.Core.VR.Models.Commons;

namespace TGS.GameProviders.ApiClients.VR
{
    internal class VRApiClient : IVRApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<VRApiClient> _logger;
        private readonly ITgsLog _tgsLog;
        private readonly IVRUtility _utility;

        public VRApiClient(
            HttpClient client,
            ILogger<VRApiClient> logger,
            ITgsLog tgsLog,
            IVRUtility utility)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
        }

        public ValueTask<VRErrorCodeResponse> CreatePlayerAsync(Uri apiUrl, string merchantid, string version, string deskey, VRCreatePlayerRequest source)
            => PostAsync<VRCreatePlayerRequest, VRErrorCodeResponse>(apiUrl, merchantid, version, deskey, VRAction.CreatePlayer, source);

        public ValueTask<VRFnudResponse> FundAsync(Uri apiUrl, string merchantid, string version, string deskey, VRAction action, VRFnudRequest source)
            => PostAsync<VRFnudRequest, VRFnudResponse>(apiUrl, merchantid, version, deskey, action, source);

        public ValueTask<VRFundCheckingResponse> FundCheckingAsync(Uri apiUrl, string merchantid, string version, string deskey, VRFundCheckingRequest source)
            => PostAsync<VRFundCheckingRequest, VRFundCheckingResponse>(apiUrl, merchantid, version, deskey, VRAction.FundChecking, source);

        public ValueTask<VRGetBalanceResponse> GetBalanceAsync(Uri apiUrl, string merchantid, string version, string deskey, VRGetBalanceRequest source)
             => PostAsync<VRGetBalanceRequest, VRGetBalanceResponse>(apiUrl, merchantid, version, deskey, VRAction.GetBalance, source);

        public ValueTask<VRGameRecordResponse> GetGameRecordAsync(Uri apiUrl, string merchantid, string version, string deskey, VRGameRecordRequest source)
            => PostAsync<VRGameRecordRequest, VRGameRecordResponse>(apiUrl, merchantid, version, deskey, VRAction.GameRecord, source);

        public Uri GetGameURL(Uri apiUrl, string merchantid, string version, string deskey, VRGameUrlRequest source)

        {
            var path = _utility.GetPath(source);

            var dataEncrypt = _utility.AesEncrypt(path, deskey, true);

            var data = new VRApiRequest()
            {
                version = version,
                id = merchantid,
                data = dataEncrypt,
            };

            return new Uri($"{apiUrl.Scheme}://{apiUrl.Host}{_utility.GetAction(VRAction.GetGameUrl)}?{_utility.GetPath(data)}");
        }

        public ValueTask<VRErrorCodeResponse> KickAsync(Uri apiUrl, string merchantid, string version, string deskey, VRKickRequest source)
             => PostAsync<VRKickRequest, VRErrorCodeResponse>(apiUrl, merchantid, version, deskey, VRAction.Kick, source);

        private async ValueTask<ResultData> PostAsync<SourceData, ResultData>(Uri apiUrl, string merchantid, string version, string deskey, VRAction action, SourceData source)
        {
            var logPrefix = $"{nameof(VRApiClient).ToLower()}.{action}";
            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(ResultData);

            var dataEncrypt = _utility.AesEncrypt(JsonSerializer.Serialize(source), deskey, false);

            var data = new VRApiRequest()
            {
                version = version,
                id = merchantid,
                data = dataEncrypt,
            };

            HttpContent content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

            var url = new Uri($"{apiUrl.Scheme}://{apiUrl.Host}{_utility.GetAction(action)}");

            var httpResponseMessage = await _client.PostAsync(url, content);
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
                RequestContent = JsonSerializer.Serialize(data),
                ResponseHeaders = httpResponseMessage.RequestMessage.Headers.ToString(),
                ResponseContent = responseMessage,
                ResponseCode = (short)httpResponseMessage.StatusCode,
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                RecordedOn = DateTimeOffset.UtcNow
            });

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                var contentDecrypt = _utility.AesDecrypt(responseMessage, deskey, false);
                if (!string.IsNullOrEmpty(contentDecrypt))
                {
                    responseMessage = contentDecrypt;
                    response = JsonSerializer.Deserialize<ResultData>(contentDecrypt);
                }
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
                    RequestContent = JsonSerializer.Serialize(data),
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
