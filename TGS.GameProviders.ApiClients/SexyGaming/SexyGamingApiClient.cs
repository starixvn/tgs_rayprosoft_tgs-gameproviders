using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.SexyGaming.Models;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.Extensions;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.ApiClients;

namespace TGS.GameProviders.ApiClients.SexyGaming
{
    internal class SexyGamingApiClient : ISexyGamingApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<SexyGamingApiClient> _logger;
        private readonly ITgsLog _tgsLog;

        public SexyGamingApiClient(
            HttpClient client,
            ILogger<SexyGamingApiClient> logger,
            ITgsLog tgsLog
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
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

        private async ValueTask<TResult> GetAPIAsync<TSource, TResult>(SexyGamingConfiguration configuration, string path, TSource source, string logPrefix)
        {
            logPrefix = $"{GameProviderType.SexyGaming}.{logPrefix}";
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

            _tgsLog.LogApi(new()
            {
                Ip = string.Empty,
                Path = httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri.NoLongerThan(512),
                Method = httpResponseMessage.RequestMessage.Method.ToString().NoLongerThan(8),
                Prefix = logPrefix,
                ServerName = Environment.MachineName.NoLongerThan(128),
                RequestHeaders = httpResponseMessage.RequestMessage.Headers + httpResponseMessage.RequestMessage.Content?.Headers?.ToString(),
                RequestContent = JsonConvert.SerializeObject(data),
                ResponseHeaders = httpResponseMessage.RequestMessage.Headers.ToString(),
                ResponseContent = responseMessage,
                ResponseCode = (short)httpResponseMessage.StatusCode,
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                RecordedOn = DateTimeOffset.UtcNow
            });

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                response = System.Text.Json.JsonSerializer.Deserialize<TResult>(responseMessage);
            }
            else
            {
                _logger.LogError(System.Text.Json.JsonSerializer.Serialize(new
                {
                    Ip = string.Empty,
                    Path = httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri.NoLongerThan(512),
                    Method = httpResponseMessage.RequestMessage.Method.ToString().NoLongerThan(8),
                    Prefix = logPrefix,
                    ServerName = Environment.MachineName.NoLongerThan(128),
                    RequestHeaders = httpResponseMessage.RequestMessage.Headers + httpResponseMessage.RequestMessage.Content?.Headers?.ToString(),
                    RequestContent = JsonConvert.SerializeObject(data),
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
