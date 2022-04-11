using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.Extensions;
using Tgs.GameProviders.Core.VR;
using Tgs.GameProviders.Core.VR.Models;
using Tgs.GameProviders.Core.VR.Models.Commons;
using Tgs.Logging;

namespace Tgs.GameProviders.ApiClients.VR
{
    internal class VRApiClient : IVRApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<VRApiClient> _logger;
        private readonly ITgsLog _tgsLog;

        public VRApiClient(
            HttpClient client,
            ILogger<VRApiClient> logger,
            ITgsLog tgsLog)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
        }

        public ValueTask<VRErrorCodeResponse> CreatePlayerAsync(Uri apiUrl, string merchantid, string version, string deskey, VRCreatePlayerRequest source)
            => PostAsync<VRCreatePlayerRequest, VRErrorCodeResponse>(apiUrl, merchantid, version, deskey, VRAction.CreatePlayer, source);

        public ValueTask<VRFnudResponse> FundAsync(Uri apiUrl, string merchantid, string version, string deskey, VRAction action, VRFnudRequest source)
            => PostAsync<VRFnudRequest, VRFnudResponse>(apiUrl, merchantid, version, deskey, action, source);

        public ValueTask<VRFundCheckingResponse> FundCheckingAsync(Uri apiUrl, string merchantid, string version, string deskey, VRFundCheckingRequest source)
            => PostAsync<VRFundCheckingRequest, VRFundCheckingResponse>(apiUrl, merchantid, version, deskey, VRAction.FundChecking, source);

        public ValueTask<VRGetBalanceResponse> GetBalanceAsync(Uri apiUrl, string merchantid, string version, string deskey, VRGetBalanceRequest source)
             => PostAsync<VRGetBalanceRequest, VRGetBalanceResponse>(apiUrl, merchantid, version, deskey, VRAction.GetBalance, source);

        public ValueTask<VRGameRecordResponse> GetGameRecordAsync(Uri apiUrl, string merchantid, string version, string deskey, VRGameRecordRequest source)
            => PostAsync<VRGameRecordRequest, VRGameRecordResponse>(apiUrl, merchantid, version, deskey, VRAction.GameRecord, source);

        public Uri GetGameURL(Uri apiUrl, string merchantid, string version, string deskey, VRGameUrlRequest source)

        {
            var path = GetPath(source);

            var dataEncrypt = AesEncrypt(path, deskey, true);

            var data = new
            {
                version = version,
                id = merchantid,
                data = dataEncrypt,
            };

            return new Uri($"{apiUrl.Scheme}://{apiUrl.Host}{GetAction(VRAction.GetGameUrl)}?{GetPath(data)}");
        }

        public ValueTask<VRErrorCodeResponse> KickAsync(Uri apiUrl, string merchantid, string version, string deskey, VRKickRequest source)
             => PostAsync<VRKickRequest, VRErrorCodeResponse>(apiUrl, merchantid, version, deskey, VRAction.Kick, source);

        private static string AesDecrypt(string data, string secretKey, bool isDecodeUrl = true)
        {
            try
            {
                if (string.IsNullOrEmpty(data))
                    return null;
                if (isDecodeUrl)
                    data = System.Web.HttpUtility.UrlDecode(data);

                byte[] dataBytes = Convert.FromBase64String(data);

                using (var aes = Aes.Create())
                {
                    aes.Mode = CipherMode.ECB;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = Encoding.UTF8.GetBytes(secretKey);
                    aes.GenerateIV();

                    using (ICryptoTransform cryptoTransform = aes.CreateDecryptor())
                    {
                        using (MemoryStream memoryStream = new MemoryStream(dataBytes))
                        {
                            using (CryptoStream csDecrypt = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read))
                            {
                                using (StreamReader streamReader = new StreamReader(csDecrypt))
                                {
                                    return streamReader.ReadToEnd();
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        private static string AesEncrypt(string data, string secretKey, bool isEncodeUrl = true)
        {
            if (string.IsNullOrEmpty(data))
                return null;
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            using (var aes = Aes.Create())
            {
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Key = Encoding.UTF8.GetBytes(secretKey);
                aes.GenerateIV();

                byte[] encrypted;
                using (var cryptoTransform = aes.CreateEncryptor())
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(dataBytes, 0, dataBytes.Length);
                            cryptoStream.FlushFinalBlock();
                            encrypted = memoryStream.ToArray();

                            var base64Str = Convert.ToBase64String(encrypted, 0, encrypted.Length);

                            return isEncodeUrl ? System.Web.HttpUtility.UrlEncode(base64Str) : base64Str;
                        }
                    }
                }
            }
        }

        private static string GetAction(VRAction action)
        {
            return VRActionMapper.Mapper.Where(x => x.Key == action).First().Value;
        }

        private static string GetPath<TRequest>(TRequest request, JsonSerializerOptions options = null)
        {
            var value = JsonSerializer.Serialize(request, options);
            var items = JsonSerializer.Deserialize<Dictionary<string, object>>(value, options);
            return string.Join("&", items.Where(x => x.Value != null).Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
        }

        private async ValueTask<ResultData> PostAsync<SourceData, ResultData>(Uri apiUrl, string merchantid, string version, string deskey, VRAction action, SourceData source)
        {
            var logPrefix = $"{nameof(VRApiClient).ToLower()}.{action}";

            var sw = Stopwatch.StartNew();

            var dataEncrypt = AesEncrypt(JsonSerializer.Serialize(source), deskey, false);

            var data = new
            {
                version = version,
                id = merchantid,
                data = dataEncrypt,
            };

            HttpContent content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

            var url = new Uri($"{apiUrl.Scheme}://{apiUrl.Host}{GetAction(action)}");

            var httpResponseMessage = await _client.PostAsync(url, content);

            sw.Stop();

            var responseMessage = default(string);
            using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result))
            {
                responseMessage = await sr.ReadToEndAsync() ?? string.Empty;
            }

            var apiLogEvent = new ApiLogEvent
            {
                Path = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
                Method = httpResponseMessage.RequestMessage?.Method.ToString().NoLongerThan(8) ?? string.Empty,
                Prefix = logPrefix,
                ServerName = Environment.MachineName.NoLongerThan(128),
                RequestHeaders = httpResponseMessage.RequestMessage?.Headers + httpResponseMessage.RequestMessage?.Content?.Headers?.ToString(),
                RequestContent = (await content.ReadAsStringAsync().ConfigureAwait(false)).NoLongerThan(4096),
                ResponseHeaders = httpResponseMessage.Headers.ToString(),
                ResponseContent = responseMessage.NoLongerThan(4096),
                ResponseCode = (short)httpResponseMessage.StatusCode,
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                RecordedOn = DateTimeOffset.UtcNow
            };
            _tgsLog.LogApi(apiLogEvent);
            _logger.LogInformation(JsonSerializer.Serialize(apiLogEvent));

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                var contentDecrypt = AesDecrypt(responseMessage, deskey, false);
                if (!string.IsNullOrEmpty(contentDecrypt))
                {
                    responseMessage = contentDecrypt;
                    return JsonSerializer.Deserialize<ResultData>(contentDecrypt);
                }
            }

            return default;
        }
    }
}
