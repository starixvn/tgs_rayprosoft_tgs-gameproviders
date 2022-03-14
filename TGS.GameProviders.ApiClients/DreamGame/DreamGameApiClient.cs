using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Tgs.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.DreamGame;
using TGS.GameProviders.Core.DreamGame.Models;
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Domains;

namespace TGS.GameProviders.ApiClients.DreamGame
{
    internal class DreamGameApiClient : IDreamGameApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<DreamGameApiClient> _logger;
        private readonly ITgsLog _tgsLog;

        public DreamGameApiClient(
            HttpClient client,
            ILogger<DreamGameApiClient> logger,
            ITgsLog tgsLog)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
        }

        public async ValueTask<LoginResult> LoginAsync(Uri apiUrl, string apiKey, string agentName, string username, string password, PlatformType platformType = PlatformType.Desktop, string lang = "en")
        {
            var random = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var token = GetHashMD5($"{agentName}{apiKey }{random}");

            var request = new
            {
                token,
                random,
                lang,
                member = new
                {
                    username,
                    password = GetHashMD5(password)
                }
            };

            var response = await PostAsync(
                apiUrl,
                $"/user/login/{agentName} ",
                JsonDocument.Parse(JsonSerializer.Serialize(request)),
                "SignUp").ConfigureAwait(false);

            var codeId = response.RootElement.GetProperty("codeId").GetInt32();

            if (codeId is not 0)
            {
                return new(false, codeId, null);
            }

            var list = response.RootElement.GetProperty("list").Deserialize<string[]>();

            var url = platformType == PlatformType.Desktop ? list[0] : list[1];
            url += $"{response.RootElement.GetProperty("token").GetString()}&language={lang}";

            return new(true, codeId, new(url));
        }

        public async ValueTask<bool> OfflineAsync(Uri apiUrl, string apiKey, string agentName, int memberId)
        {

            var tryCount = 1;
            var seconds = 30;
            var random = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var token = GetHashMD5($"{agentName}{apiKey }{random}");
            var request = new
            {
                token,
                random,
                list = new[] { memberId }
            };

            var retryPolicy = Policy.HandleResult<JsonDocument>(response
                 => response.RootElement.GetProperty("codeId").GetInt32() == 405)
                .WaitAndRetryAsync(tryCount,
                 retryAttempt => TimeSpan.FromSeconds(seconds),
                 (response, retryTime, context) =>
                 {
                     var data = new
                     {
                         GameProviderCode = GameProviderType.DreamGame,
                         Response = response,
                         Delay = retryTime,
                         CreationTime = DateTimeOffset.UtcNow
                     };
                     _logger.LogWarning(JsonSerializer.Serialize(data));
                 });

            var response = await retryPolicy.ExecuteAsync(async ()
                 => await PostAsync(
                     apiUrl,
                     $"/user/offline/{agentName} ",
                     JsonDocument.Parse(JsonSerializer.Serialize(request)),
                     "Offline").ConfigureAwait(false)).ConfigureAwait(false);

            return response.RootElement.GetProperty("codeId").GetInt32() == 0;
        }

        public async ValueTask<IEnumerable<OnlineReportResult>> OnlineReportAsync(Uri apiUrl, string apiKey, string agentName)
        {

            var tryCount = 1;
            var seconds = 30;
            var random = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var token = GetHashMD5($"{agentName}{apiKey }{random}");
            var request = new
            {
                token,
                random
            };

            var retryPolicy = Policy.HandleResult<JsonDocument>(response
                 => response.RootElement.GetProperty("codeId").GetInt32() == 405)
             .WaitAndRetryAsync(tryCount,
                 retryAttempt => TimeSpan.FromSeconds(seconds),
                 (response, retryTime, context) =>
                 {
                     var data = new
                     {
                         GameProviderCode = GameProviderType.DreamGame,
                         Response = response,
                         Delay = retryTime,
                         CreationTime = DateTimeOffset.UtcNow
                     };
                     _logger.LogWarning(System.Text.Json.JsonSerializer.Serialize(data));
                 });

            var response = await retryPolicy.ExecuteAsync(async ()
                => await PostAsync(
                    apiUrl,
                    $"/user/onlineReport/{agentName} ",
                    JsonDocument.Parse(JsonSerializer.Serialize(request)),
                    "OnlineReport").ConfigureAwait(false)).ConfigureAwait(false);


            var codeId = response.RootElement.GetProperty("codeId").GetInt32();

            if (codeId == 0)
            {
                if (response.RootElement.TryGetProperty(("list"), out JsonElement list))
                {
                    return list.Deserialize<IEnumerable<OnlineReportResult>>();
                }
            }

            return null;
        }

        public async ValueTask<int> SignUpAsync(Uri apiUrl, string apiKey, string agentName, string username, string password, string currencyName, string tableLimit = "A", int winLimit = 0)
        {
            var random = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var token = GetHashMD5($"{agentName}{apiKey }{random}");

            var request = new
            {
                token,
                random,
                data = tableLimit,
                member = new
                {
                    username,
                    password = GetHashMD5(password),
                    currencyName,
                    winLimit
                }
            };

            var response = await PostAsync(
                apiUrl,
                $"/user/signup/{agentName} ",
                JsonDocument.Parse(JsonSerializer.Serialize(request)),
                "SignUp").ConfigureAwait(false);
            return response.RootElement.GetProperty("codeId").GetInt32();
        }

        private static string GetHashMD5(string data)
        {
            using var md5 = MD5.Create();
            var dataByte = Encoding.UTF8.GetBytes(data);
            var hash = md5.ComputeHash(dataByte);
            return hash.ToHexString().ToUpper();
        }

        private async ValueTask<JsonDocument> PostAsync(Uri apiUrl, string paths, JsonDocument request, string logPrefix)
        {
            logPrefix = $"{nameof(DreamGameApiClient).ToLower() }.{logPrefix}";
            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);

            var url = $"{apiUrl?.ToString()}?{paths}";

            HttpContent content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var httpResponseMessage = await _client.PostAsync($"{apiUrl?.ToString()}{paths}", content);
            sw.Stop();
            using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result))
            {
                responseMessage = await sr.ReadToEndAsync() ?? string.Empty;
            }

            sw.Stop();

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
                return JsonDocument.Parse(responseMessage);
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

            return null;
        }
    }
}
