using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.KY.Models;
using TGS.GameProviders.ApiClients.KY.Models.Commons;
using TGS.GameProviders.Domains.SettingConfiguration.KY;

namespace TGS.GameProviders.ApiClients.KY
{
    internal class KYApiClient : IKYApiClient
    {
        private readonly HttpClient _client;
        private readonly IKYUtility _utility;

        public KYApiClient(
            HttpClient client,
            IKYUtility utility)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
        }

        public ValueTask<KYApiResponse<KYFnudResponse>> FundAsync(KYBrandMerchant brandMerchant, string agentId, DateTimeOffset dateTimeOffset, KYAction action, KYFnudRequest request)
        {
            request.Action = _utility.GetAction(action) ?? -1;

            var timestamp = dateTimeOffset.ToUnixTimeMilliseconds();
            var dataPath = _utility.GetPath(request);
            var param = _utility.AesEncrypt(dataPath, brandMerchant.Deskey, true);
            var plainText = $"{agentId}{timestamp}{brandMerchant.Md5key}";
            var key = _utility.HashMD5(plainText);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<KYFnudResponse>(brandMerchant.ApiUrl, action, apiKYRequest);
        }

        public ValueTask<KYApiResponse<KYFundCheckingResponse>> FundCheckingAsync(KYBrandMerchant brandMerchant, string agentId, KYFundCheckingRequest request)
        {
            var action = KYAction.FundChecking;
            request.Action = _utility.GetAction(action) ?? -1;

            var timestamp = DateTimeOffset.UtcNow.AddHours(+8).ToUnixTimeMilliseconds();
            var dataPath = _utility.GetPath(request);
            var param = _utility.AesEncrypt(dataPath, brandMerchant.Deskey, true);
            var plainText = $"{agentId}{timestamp}{brandMerchant.Md5key}";
            var key = _utility.HashMD5(plainText);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<KYFundCheckingResponse>(brandMerchant.ApiUrl, action, apiKYRequest);
        }

        public ValueTask<KYApiResponse<KYGetBalanceResponse>> GetBalanceAsync(KYBrandMerchant brandMerchant, string agentId, KYGetBalanceRequest request)
        {
            var action = KYAction.GetBalance;
            request.Action = _utility.GetAction(action) ?? -1;

            var timestamp = DateTimeOffset.UtcNow.AddHours(+8).ToUnixTimeMilliseconds();
            var dataPath = _utility.GetPath(request);
            var param = _utility.AesEncrypt(dataPath, brandMerchant.Deskey, true);
            var plainText = $"{agentId}{timestamp}{brandMerchant.Md5key}";
            var key = _utility.HashMD5(plainText);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<KYGetBalanceResponse>(brandMerchant.ApiUrl, action, apiKYRequest);
        }

        public ValueTask<KYApiResponse<KYGameRecordResponse>> GetGameRecordAsync(KYBrandMerchant brandMerchant, string agentId, DateTimeOffset dateTimeOffset, KYGameRecordRequest request)
        {
            var action = KYAction.GameRecord;
            request.Action = _utility.GetAction(action) ?? -1;

            var timestamp = dateTimeOffset.ToUnixTimeMilliseconds();
            var dataPath = _utility.GetPath(request);
            var param = _utility.AesEncrypt(dataPath, brandMerchant.Deskey, true);
            var plainText = $"{agentId}{timestamp}{brandMerchant.Md5key}";
            var key = _utility.HashMD5(plainText);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<KYGameRecordResponse>(brandMerchant.ApiRecordUrl, action, apiKYRequest);
        }

        public ValueTask<KYApiResponse<KYGameUrlResponse>> GetGameUrlAsync(KYBrandMerchant brandMerchant, string agentId, DateTimeOffset dateTimeOffset, KYGameUrlRequest request)
        {
            var action = KYAction.GetGameUrl;
            request.Action = _utility.GetAction(KYAction.GetGameUrl) ?? -1;

            var timestamp = dateTimeOffset.ToUnixTimeMilliseconds();
            var dataPath = _utility.GetPath(request);
            var param = _utility.AesEncrypt(dataPath, brandMerchant.Deskey, true);
            var plainText = $"{agentId}{timestamp}{brandMerchant.Md5key}";
            var key = _utility.HashMD5(plainText);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<KYGameUrlResponse>(brandMerchant.ApiUrl, action, apiKYRequest);
        }

        public ValueTask<KYApiResponse<KYKickResponse>> KickAsync(KYBrandMerchant brandMerchant, string agentId, KYKickRequest request)
        {
            var action = KYAction.Kick;
            request.Action = _utility.GetAction(action) ?? -1;

            var timestamp = DateTimeOffset.UtcNow.AddHours(+8).ToUnixTimeMilliseconds();
            var dataPath = _utility.GetPath(request);
            var param = _utility.AesEncrypt(dataPath, brandMerchant.Deskey, true);
            var plainText = $"{agentId}{timestamp}{brandMerchant.Md5key}";
            var key = _utility.HashMD5(plainText);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<KYKickResponse>(brandMerchant.ApiUrl, action, apiKYRequest);
        }

        private async ValueTask<KYApiResponse<TData>> GetAPIAsync<TData>(Uri apiUrl, KYAction action, KYApiRequest request)
        {
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

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                response = JsonSerializer.Deserialize<KYApiResponse<TData>>(responseMessage);
            }

            return response;
        }
    }
}
