using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TGS.GameProviders.ApiClients.SexyGaming.Models;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.ApiClients.SexyGaming
{
    internal class SexyGamingApiClient : ISexyGamingApiClient
    {
        private readonly HttpClient _client;

        public SexyGamingApiClient(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<SexyGamingCreateMemberResponse> CreatePlayerAsync(SexyGamingConfiguration configuration, SexyGamingCreateMemberRequest request)
        {
            request.AgentId = configuration.AgentId;
            request.Cert = configuration.Cert;
            return await GetAPIAsync<SexyGamingCreateMemberRequest, SexyGamingCreateMemberResponse>(configuration, "/wallet/createMember", request, $"CreatePlayerAsync").ConfigureAwait(false);
        }

        public async Task<SexyGamingGameUrlResponse> GetGameUrlAsync(SexyGamingConfiguration configuration, SexyGamingGameUrlRequest request)
        {
            request.AgentId = configuration.AgentId;
            request.Cert = configuration.Cert;
            return await GetAPIAsync<SexyGamingGameUrlRequest, SexyGamingGameUrlResponse>(configuration, "/wallet/login", request, "GetGameUrl").ConfigureAwait(false);
        }

        public async Task<SexyGamingGameUrlResponse> GetGameUrlAsync(SexyGamingConfiguration configuration, SexyGamingLoginGameRequest request)
        {
            request.AgentId = configuration.AgentId;
            request.Cert = configuration.Cert;
            return await GetAPIAsync<SexyGamingLoginGameRequest, SexyGamingGameUrlResponse>(configuration, "/wallet/doLoginAndLaunchGame", request, "GetGameUrl").ConfigureAwait(false);
        }

        public async Task<SexyGamingLogoutResponse> LogoutAsync(SexyGamingConfiguration configuration, SexyGamingLogoutRequest request)
        {
            request.AgentId = configuration.AgentId;
            request.Cert = configuration.Cert;
            return await GetAPIAsync<SexyGamingLogoutRequest, SexyGamingLogoutResponse>(configuration, "/wallet/logout", request, "LogOut").ConfigureAwait(false);
        }

        private async ValueTask<TResult> GetAPIAsync<TSource, TResult>(SexyGamingConfiguration configuration, string path, TSource source, string prefix)
        {
            prefix = $"{GameProviderType.SexyGaming}{prefix}";
            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(TResult);

            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(System.Text.Json.JsonSerializer.Serialize(source));

            var url = new Uri($"{configuration.ApiUrl.Scheme}://{configuration.ApiUrl.Host}{path}");

            HttpContent content = new FormUrlEncodedContent(data);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            content.Headers.ContentType.CharSet = "UTF-8";

            var httpResponseMessage = await _client.PostAsync(url, content);
            sw.Stop();
            using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result))
            {
                responseMessage = await sr.ReadToEndAsync() ?? string.Empty;
            }

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                response = System.Text.Json.JsonSerializer.Deserialize<TResult>(responseMessage);
            }

            return response;
        }
    }
}
