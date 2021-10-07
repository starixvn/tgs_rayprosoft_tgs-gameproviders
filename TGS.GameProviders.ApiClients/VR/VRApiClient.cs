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
using TGS.GameProviders.ApiClients.VR.Models;
using TGS.GameProviders.ApiClients.VR.Models.Commons;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.Extensions;
using TGS.GameProviders.Domains.SettingConfiguration.VR;

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

        public ValueTask<VRErrorCodeResponse> CreatePlayerAsync(VRBrandMerchant brandMerchant, VRCreatePlayerRequest request)
            => GetAPIAsync<VRCreatePlayerRequest, VRErrorCodeResponse>(brandMerchant, VRAction.CreatePlayer, request);

        public ValueTask<VRFnudResponse> FundAsync(VRBrandMerchant brandMerchant, VRAction action, VRFnudRequest request)
            => GetAPIAsync<VRFnudRequest, VRFnudResponse>(brandMerchant, action, request);

        public ValueTask<VRFundCheckingResponse> FundCheckingAsync(VRBrandMerchant brandMerchant, VRFundCheckingRequest request)
            => GetAPIAsync<VRFundCheckingRequest, VRFundCheckingResponse>(brandMerchant, VRAction.FundChecking, request);

        public ValueTask<VRGetBalanceResponse> GetBalanceAsync(VRBrandMerchant brandMerchant, VRGetBalanceRequest request)
             => GetAPIAsync<VRGetBalanceRequest, VRGetBalanceResponse>(brandMerchant, VRAction.GetBalance, request);

        public ValueTask<VRGameRecordResponse> GetGameRecordAsync(VRBrandMerchant brandMerchant, VRGameRecordRequest request)
            => GetAPIAsync<VRGameRecordRequest, VRGameRecordResponse>(brandMerchant, VRAction.GameRecord, request);

        public Uri GetGameURLAsync(VRBrandMerchant brandMerchant, VRGameUrlRequest request)

        {
            var path = _utility.GetPath(request);

            var dataEncrypt = _utility.AesEncrypt(path, brandMerchant.Deskey, true);

            var apiUrl = brandMerchant.ApiUrl;

            var data = new VRApiRequest()
            {
                version = brandMerchant.Version,
                id = brandMerchant.Merchantid,
                data = dataEncrypt,
            };

            return new Uri($"{apiUrl.Scheme}://{apiUrl.Host}{_utility.GetAction(VRAction.GetGameUrl)}?{_utility.GetPath(data)}");
        }

        public ValueTask<VRErrorCodeResponse> KickAsync(VRBrandMerchant brandMerchant, VRKickRequest request)
             => GetAPIAsync<VRKickRequest, VRErrorCodeResponse>(brandMerchant, VRAction.Kick, request);

        private async ValueTask<ResultData> GetAPIAsync<SourceData, ResultData>(VRBrandMerchant brandMerchant, VRAction action, SourceData request)
        {
            var logPrefix = $"{GameProviderType.VR}.{action}";
            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(ResultData);

            var dataEncrypt = _utility.AesEncrypt(JsonSerializer.Serialize(request), brandMerchant.Deskey, false);

            var apiUrl = brandMerchant.ApiUrl;

            var data = new VRApiRequest()
            {
                version = brandMerchant.Version,
                id = brandMerchant.Merchantid,
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
                var contentDecrypt = _utility.AesDecrypt(responseMessage, brandMerchant.Deskey, false);
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
