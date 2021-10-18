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
using TGS.GameProviders.ApiClients.TCG.Models;
using TGS.GameProviders.ApiClients.TCG.Models.Commons;
using TGS.GameProviders.Domains.Extensions;

namespace TGS.GameProviders.ApiClients.TCG
{
    internal class TCGApiClient : ITCGApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<TCGApiClient> _logger;
        private readonly ITgsLog _tgsLog;
        private readonly ITCGUtility _utility;

        public TCGApiClient(
            HttpClient client,
            ILogger<TCGApiClient> logger,
            ITgsLog tgsLog,
            ITCGUtility utility)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
        }

        public ValueTask<TCGKickOutLottoPlayerResponse> KickOutLottoPlayerAsync(Uri apiUrl, string merchantCode, string desKey, string sha256Key, TCGKickOutLottoPlayerRequest request)
            => PostAsync<TCGKickOutLottoPlayerRequest, TCGKickOutLottoPlayerResponse>(apiUrl, merchantCode, desKey, sha256Key, request, "KickOutLottoPlayer");

        public ValueTask<TCGLaunchGameResponse> LaunchGameAsync(Uri apiUrl, string merchantCode, string desKey, string sha256Key, TCGLaunchGameByELOTTORequest request)
            => PostAsync<TCGLaunchGameByELOTTORequest, TCGLaunchGameResponse>(apiUrl, merchantCode, desKey, sha256Key, request, "LaunchGameByELOTTO");

        public ValueTask<TCGLaunchGameResponse> LaunchGameAsync(Uri apiUrl, string merchantCode, string desKey, string sha256Key, TCGLaunchGameBySEARequest request)
            => PostAsync<TCGLaunchGameBySEARequest, TCGLaunchGameResponse>(apiUrl, merchantCode, desKey, sha256Key, request, "LaunchGameBySEA");

        public ValueTask<TCGLaunchGameResponse> LaunchGameAsync(Uri apiUrl, string merchantCode, string desKey, string sha256Key, TCGLaunchGameByLOTTOVNRequest request)
            => PostAsync<TCGLaunchGameByLOTTOVNRequest, TCGLaunchGameResponse>(apiUrl, merchantCode, desKey, sha256Key, request, "LaunchGameByLOTTOVN");

        public ValueTask<TCGRegisterPlayerResponse> RegisterPlayerAsync(Uri apiUrl, string merchantCode, string desKey, string sha256Key, TCGRegisterPlayerRequest request)
            => PostAsync<TCGRegisterPlayerRequest, TCGRegisterPlayerResponse>(apiUrl, merchantCode, desKey, sha256Key, request, "RegisterPlayer");

        public ValueTask<TCGUpdatePasswordPlayerResponse> RegisterPlayerAsync(Uri apiUrl, string merchantCode, string desKey, string sha256Key, TCGUpdatePasswordPlayerRequest request)
            => PostAsync<TCGUpdatePasswordPlayerRequest, TCGUpdatePasswordPlayerResponse>(apiUrl, merchantCode, desKey, sha256Key, request, "RegisterPlayer");

        private async ValueTask<TResponse> PostAsync<TRequest, TResponse>(Uri apiUrl, string merchantCode, string desKey, string sha256Key, TRequest request, string logPrefix) where TResponse : TCGBaseResponse
        {
            logPrefix = $"{nameof(TCGApiClient).ToLower() }.{logPrefix}";
            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(TResponse);
            var jsonString = JsonSerializer.Serialize(request);
            var encryptedParams = _utility.DESCEncrypt(jsonString, desKey);
            var sign = _utility.GetHash256(encryptedParams, sha256Key);
            var data =
                "merchant_code=" + WebUtility.UrlEncode(merchantCode)
                + "&params=" + WebUtility.UrlEncode(encryptedParams)
                + "&sign=" + WebUtility.UrlEncode(sign);

            HttpContent content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");

            var httpResponseMessage = await _client.PostAsync(apiUrl, content);
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
                response = JsonSerializer.Deserialize<TResponse>(responseMessage);
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
