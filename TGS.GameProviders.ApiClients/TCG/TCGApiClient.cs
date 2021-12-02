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
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Core.TCG;
using TGS.GameProviders.Core.TCG.Models;
using TGS.GameProviders.Core.TCG.Models.Commons;

namespace TGS.GameProviders.ApiClients.TCG
{
    internal class TCGApiClient : ITCGApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<TCGApiClient> _logger;
        private readonly ITgsLog _tgsLog;

        public TCGApiClient(
            HttpClient client,
            ILogger<TCGApiClient> logger,
            ITgsLog tgsLog)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
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

        public ValueTask<TCGUpdatePasswordPlayerResponse> UpdatePasswordPlayerAsync(Uri apiUrl, string merchantCode, string desKey, string sha256Key, TCGUpdatePasswordPlayerRequest request)
            => PostAsync<TCGUpdatePasswordPlayerRequest, TCGUpdatePasswordPlayerResponse>(apiUrl, merchantCode, desKey, sha256Key, request, "RegisterPlayer");

        private async ValueTask<TResponse> PostAsync<TRequest, TResponse>(Uri apiUrl, string merchantCode, string desKey, string sha256Key, TRequest request, string logPrefix) where TResponse : TCGBaseResponse
        {
            logPrefix = $"{nameof(TCGApiClient).ToLower() }.{logPrefix}";
            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(TResponse);
            var jsonString = JsonSerializer.Serialize(request);
            var encryptedParams = GetDESCEncrypt(jsonString, desKey);
            var sign = GetHash256(encryptedParams, sha256Key);
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

        private string DESCDecrypt(string data, string key)
        {
            if (string.IsNullOrEmpty(data))
                return null;

            Byte[] toEncryptArray = Convert.FromBase64String(data);

            using (var dESCrypto = DES.Create())
            {
                dESCrypto.Key = Encoding.UTF8.GetBytes(key);
                dESCrypto.Mode = CipherMode.ECB;
                dESCrypto.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = dESCrypto.CreateDecryptor();
                var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return Encoding.UTF8.GetString(resultArray);
            }
        }

        private string GetDESCEncrypt(string data, string key)
        {
            if (string.IsNullOrEmpty(data))
                return null;

            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            using (var des = DES.Create())
            {
                des.Key = Encoding.UTF8.GetBytes(key);
                des.Mode = CipherMode.ECB;
                des.Padding = PaddingMode.PKCS7;
                using (var cryptoTransform = des.CreateEncryptor())
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(dataBytes, 0, dataBytes.Length);
                            cryptoStream.FlushFinalBlock();
                            var encrypted = memoryStream.ToArray();
                            return Convert.ToBase64String(encrypted, 0, encrypted.Length);
                        }
                    }
                }
            }
        }

        private string GetHash256(string data, string hash_key)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(data + hash_key);
                byte[] outputBytes = sha256.ComputeHash(inputBytes);
                return BitConverter.ToString(outputBytes).Replace("-", "").ToLower();
            }
        }
    }
}
