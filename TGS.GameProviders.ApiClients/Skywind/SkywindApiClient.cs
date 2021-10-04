using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.Skywind.Models;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.ApiClients.Skywind
{
    public class SkywindApiClient : ISkywindApiClient
    {

        private readonly HttpClient _client;
        public SkywindApiClient(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async ValueTask<SkywindGameUrlResponse> GetGameUrlAsync(SkywindConfiguration configuration, string getaccessToken, SkywindGameUrlRequest request)
        {
            var apiUrl = new Uri($"{configuration.ApiUrl}/players/{request.PlayerCode}/games/{request.GameCode}?ticket={request.Ticket}&language={request.Language}");

            return await GetAsync<SkywindGameUrlResponse>(apiUrl, getaccessToken, "UserLogin").ConfigureAwait(false);
        }

        public async ValueTask<SkywindGamHistoryRoundDetailsResponse> GetGamHistoryRoundDetailsAsync(SkywindConfiguration configuration, string getaccessToken, SkywindGamHistoryRoundDetailsRequest request)
        {
            var apiUrl = new Uri($"{configuration.ApiUrl}/history/game/{request.RoundId}/image?language={request.Language}");

            return await GetAsync<SkywindGamHistoryRoundDetailsResponse>(apiUrl, getaccessToken, "GamHistoryRoundDetails").ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the user login asynchronous.
        /// AccessToken 有效時間:一個小时了
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public async ValueTask<SkywindUserLoginResponse> GetAccessTokenAsync(SkywindConfiguration configuration)
        {
            var apiUrl = new Uri($"{configuration.ApiUrl}/login");

            var source = new SkywindUserLoginRequest()
            {
                SecretKey = configuration.SecretKey,
                Username = configuration.Username,
                Password = configuration.Password
            };
            return await PostAsync<SkywindUserLoginRequest, SkywindUserLoginResponse>(apiUrl, source, "UserLogin").ConfigureAwait(false);
        }

        private async ValueTask<TResult> GetAsync<TResult>(Uri apiUrl, string getaccessToken, string prefix)
        {
            prefix = $"{GameProviderType.Skywind }.{prefix}";

            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(TResult);
            _client.DefaultRequestHeaders.Add("X-ACCESS-TOKEN", getaccessToken);
            var httpResponseMessage = await _client.GetAsync(apiUrl);
            sw.Stop();
            using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result))
            {
                responseMessage = await sr.ReadToEndAsync() ?? string.Empty;
            }

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                response = JsonSerializer.Deserialize<TResult>(responseMessage);
            }

            return response;
        }

        private async ValueTask<TResult> PostAsync<TSource, TResult>(Uri apiUrl, TSource source, string prefix)
        {
            prefix = $"{GameProviderType.Skywind }.{prefix}";

            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(TResult);
            HttpContent content = new StringContent(JsonSerializer.Serialize(source), Encoding.UTF8, "application/json");
            var httpResponseMessage = await _client.PostAsync(apiUrl, content);
            sw.Stop();
            using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result))
            {
                responseMessage = await sr.ReadToEndAsync() ?? string.Empty;
            }

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                response = JsonSerializer.Deserialize<TResult>(responseMessage);
            }

            return response;
        }
    }
}
