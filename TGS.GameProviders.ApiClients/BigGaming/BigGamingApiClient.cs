using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.BigGaming.Models;
using TGS.GameProviders.ApiClients.BigGaming.Models.Commons;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.Extensions;
using TGS.GameProviders.Domains.SettingConfiguration.BigGaming;

namespace TGS.GameProviders.ApiClients.BigGaming
{
    internal class BigGamingApiClient : IBigGamingApiClient
    {
        private readonly HttpClient _client;
        private readonly IBigGamingUtility _utility;

        public BigGamingApiClient(HttpClient client, IBigGamingUtility utility)

        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
        }

        public async ValueTask<BigGamingResponse> CreateAgentAsync(BigGamingBrandMerchant brandMerchant, BigGamingCreateAgentRequest request)
        {
            request.Sign = _utility.GetHashMD5($"{request.Random }{brandMerchant.SecretCode}{request.LoginId}{brandMerchant.SecretKey}");
            var prefix = $"CreateAgent";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.agent.create", prefix).ConfigureAwait(false);
        }

        public async ValueTask<BigGamingResponse> CreatePlayerAsync(BigGamingBrandMerchant brandMerchant, BigGamingCreatePlayerRequest request)
        {
            request.Digest = _utility.GetHashMD5($"{request.Random }{brandMerchant.SecretCode}{_utility.GetSecretCode(brandMerchant.AgentLoginIdPassword)}");
            var prefix = $"CreatePlayer";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.user.create", prefix).ConfigureAwait(false);
        }

        public async ValueTask<BigGamingResponse> GetGameRecordAsync(BigGamingBrandMerchant brandMerchant, BigGamingGameRecordFishingRequest request)
        {
            request.Sign = _utility.GetHashMD5($"{request.Random }{brandMerchant.SecretCode}{brandMerchant.SecretKey}");
            var prefix = $"GetGameRecordFishing";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.order.bg.query", prefix).ConfigureAwait(false);
        }

        public async ValueTask<BigGamingResponse> GetGameRecordFishingUrlAsync(BigGamingBrandMerchant brandMerchant, BigGamingGameRecordFishingUrlRequest request)
        {
            request.Sign = _utility.GetHashMD5($"{request.Random }{brandMerchant.SecretCode}{request.ExternalBetId}{brandMerchant.SecretKey}");
            var prefix = $"GetGameRecordFishingUrl";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.sn.bg.order.detail.url.get", prefix).ConfigureAwait(false);
        }

        public async ValueTask<BigGamingResponse> GetGameUrlAsync(BigGamingBrandMerchant brandMerchant, BigGamingGetGameUrlLiveDealerRequest request)
        {
            request.Digest = _utility.GetHashMD5($"{request.Random }{brandMerchant.SecretCode}{request.LoginId}{_utility.GetSecretCode(brandMerchant.AgentLoginIdPassword)}");
            var prefix = $"GetGameUrlLiveDealer";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.video.game.url", prefix).ConfigureAwait(false);
        }

        public async ValueTask<BigGamingResponse> GetGameUrlAsync(BigGamingBrandMerchant brandMerchant, BigGamingGetGameUrlFishingRequest request)
        {
            request.Sign = _utility.GetHashMD5($"{request.Random }{brandMerchant.SecretCode}{brandMerchant.SecretKey}");
            var prefix = $"GetGameUrlFish";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.game.bg.url", prefix).ConfigureAwait(false);
        }

        public async ValueTask<BigGamingResponse> GetTrialGameUrlAsync(BigGamingBrandMerchant brandMerchant, BigGamingGetTrialGameUrlRequest request)
        {
            request.Sign = _utility.GetHashMD5($"{request.Random }{brandMerchant.SecretCode}{brandMerchant.SecretKey}");
            var prefix = $"GetTrialGameUrl";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.video.trial.game.url", prefix).ConfigureAwait(false);
        }

        public async ValueTask<BigGamingResponse> GetVideoOrderDetailAsync(BigGamingBrandMerchant brandMerchant, BigGamingVideoOrderDetailRequest request)
        {
            request.Sign = _utility.GetHashMD5($"{request.Random }{request.Sn}{request.OrderId}{brandMerchant.SecretKey}");
            var prefix = $"VideoOrderDetail";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.sn.video.order.detail.url.get", prefix).ConfigureAwait(false);
        }

        public async ValueTask<BigGamingResponse> GetVideoOrderQueryAsync(BigGamingBrandMerchant brandMerchant, BigGamingVideoOrderQueryRequest request)
        {
            request.Sign = _utility.GetHashMD5($"{request.Random }{request.Sn}{brandMerchant.SecretKey}");
            var prefix = $"VideoOrderQuery";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.order.query", prefix).ConfigureAwait(false);
        }

        public async ValueTask<BigGamingResponse> GetVideoRoundQueryAsync(BigGamingBrandMerchant brandMerchant, BigGamingVideoRoundQueryRequest request)
        {
            request.Sign = _utility.GetHashMD5($"{request.Random }{request.Sn}{brandMerchant.SecretKey}");
            var prefix = $"VideoRoundQuery";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.video.round.query", prefix).ConfigureAwait(false);
        }

        public async ValueTask<BigGamingResponse> InfoPlayerAsync(BigGamingBrandMerchant brandMerchant, BigGamingInfoPlayerRequest request)
        {
            request.Digest = _utility.GetHashMD5($"{request.Random }{request.Sn}{request.LoginId}{_utility.GetSecretCode(brandMerchant.AgentLoginIdPassword)}");
            var prefix = $"InfoPlayer";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.user.get", prefix).ConfigureAwait(false);
        }

        public async ValueTask<BigGamingResponse> KickPlayerAsync(BigGamingBrandMerchant brandMerchant, BigGamingKickPlayerRequest request)
        {
            request.Digest = _utility.GetHashMD5($"{request.Random }{request.Sn}{request.LoginId}{_utility.GetSecretCode(brandMerchant.AgentLoginIdPassword)}");
            var prefix = $"KickPlayer";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.user.logout", prefix).ConfigureAwait(false);
        }

        private async ValueTask<BigGamingResponse> GetAPIAsync<TSource>(Uri apiUrl, TSource source, string method, string prefix)
        {
            prefix = $"{GameProviderType.BigGaming }.{prefix}";
            var request = new BigGamingRequest()
            {
                Id = GuidExtensions.NewSequentialGuid().ToString(),
                Method = method,
                Params = JsonDocument.Parse(JsonSerializer.Serialize(source)),
                Jsonrpc = "2.0"
            };

            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(BigGamingResponse);

            HttpContent content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var httpResponseMessage = await _client.PostAsync(apiUrl, content);
            sw.Stop();
            using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result))
            {
                responseMessage = await sr.ReadToEndAsync() ?? string.Empty;
            }

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                response = JsonSerializer.Deserialize<BigGamingResponse>(responseMessage);
            }

            return response;
        }
    }
}
