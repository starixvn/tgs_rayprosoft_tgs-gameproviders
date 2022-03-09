using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.DreamGame;
using TGS.GameProviders.Core.DreamGame.Models;
using TGS.GameProviders.Core.Extensions;

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
            url += $"{response.RootElement.GetProperty("token").GetString()}&&language={lang}";

            return new(true, codeId, new(url));
        }

        public ValueTask OfflineAsync(Uri apiUrl, string apiKey, string agentName, string username) => throw new NotImplementedException();

        public ValueTask OnlineReportAsync(Uri apiUrl, string apiKey, string agentName) => throw new NotImplementedException();

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
