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
using TGS.GameProviders.Domains.Extensions;

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

        public ValueTask<SexyGamingCreateMemberResponse> CreatePlayerAsync(Uri apiUrl, SexyGamingCreateMemberRequest request)
            => PostAsync<SexyGamingCreateMemberRequest, SexyGamingCreateMemberResponse>(apiUrl, "/wallet/createMember", request, $"CreatePlayerAsync");

        public ValueTask<SexyGamingGameUrlResponse> GetGamingGameUrlAsync(Uri apiUrl, SexyGamingGameUrlRequest request)
            => PostAsync<SexyGamingGameUrlRequest, SexyGamingGameUrlResponse>(apiUrl, "/wallet/login", request, "GetGamingGameUrlAsync");

        public ValueTask<SexyGamingGameUrlResponse> GetGamingLoginGameAsync(Uri apiUrl, SexyGamingLoginGameRequest request)
            => PostAsync<SexyGamingLoginGameRequest, SexyGamingGameUrlResponse>(apiUrl, "/wallet/doLoginAndLaunchGame", request, "GetGamingLoginGameAsync");

        public ValueTask<SexyGamingLogoutResponse> LogoutAsync(Uri apiUrl, SexyGamingLogoutRequest request)
            => PostAsync<SexyGamingLogoutRequest, SexyGamingLogoutResponse>(apiUrl, "/wallet/logout", request, "LogOut");

        private async ValueTask<TResult> PostAsync<TSource, TResult>(Uri apiUrl, string path, TSource source, string logPrefix)
        {
            logPrefix = $"{nameof(SexyGamingApiClient)}.{logPrefix}";
            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(TResult);

            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(System.Text.Json.JsonSerializer.Serialize(source));

            var url = new Uri($"{apiUrl.Scheme}://{apiUrl.Host}{path}");

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
