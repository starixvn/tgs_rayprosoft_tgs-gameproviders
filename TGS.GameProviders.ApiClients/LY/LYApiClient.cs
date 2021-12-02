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
using Tgs.Logging;
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Core.LY;
using TGS.GameProviders.Core.LY.Models;
using TGS.GameProviders.Core.LY.Models.Commons;

namespace TGS.GameProviders.ApiClients.LY
{
    internal class LYApiClient : ILYApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<LYApiClient> _logger;
        private readonly ITgsLog _tgsLog;

        public LYApiClient(
            HttpClient client,
            ILogger<LYApiClient> logger,
            ITgsLog tgsLog)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
        }

        public ValueTask<LYApiResponse<LYFnudResponse>> FundAsync(Uri apiUrl, string deskey, string md5key, string agentId, DateTimeOffset dateTimeOffset, LYAction action, LYFnudRequest request)
        {
            request.Action = GetAction(action);

            var timestamp = dateTimeOffset.ToUnixTimeMilliseconds();
            var dataPath = GetPath(request);
            var param = GetAesEncrypt(dataPath, deskey);
            var plainText = $"{agentId}{timestamp}{md5key}";
            var key = GetHashMD5(plainText);

            var apiLYRequest = new LYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<LYFnudResponse>(apiUrl, action, apiLYRequest, "Fund");
        }

        public ValueTask<LYApiResponse<LYFundCheckingResponse>> FundCheckingAsync(Uri apiUrl, string deskey, string md5key, string agentId, LYFundCheckingRequest request)
        {
            var action = LYAction.FundChecking;
            request.Action = GetAction(action);

            var timestamp = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeMilliseconds();
            var dataPath = GetPath(request);
            var param = GetAesEncrypt(dataPath, deskey);
            var plainText = $"{agentId}{timestamp}{md5key}";
            var key = GetHashMD5(plainText);

            var apiLYRequest = new LYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<LYFundCheckingResponse>(apiUrl, action, apiLYRequest, "FundChecking");
        }

        public ValueTask<LYApiResponse<LYGetBalanceResponse>> GetBalanceAsync(Uri apiUrl, string deskey, string md5key, string agentId, LYGetBalanceRequest request)
        {
            var action = LYAction.GetBalance;
            request.Action = GetAction(action);

            var timestamp = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeMilliseconds();
            var dataPath = GetPath(request);
            var param = GetAesEncrypt(dataPath, deskey);
            var plainText = $"{agentId}{timestamp}{md5key}";
            var key = GetHashMD5(plainText);

            var apiLYRequest = new LYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<LYGetBalanceResponse>(apiUrl, action, apiLYRequest, "Balance");
        }

        public ValueTask<LYApiResponse<LYGameRecordResponse>> GetGameRecordAsync(Uri apiRecordUrl, string deskey, string md5key, string agentId, DateTimeOffset dateTimeOffset, LYGameRecordRequest request)
        {
            var action = LYAction.GameRecord;
            request.Action = GetAction(action);

            var timestamp = dateTimeOffset.ToUnixTimeMilliseconds();
            var dataPath = GetPath(request);
            var param = GetAesEncrypt(dataPath, deskey);
            var plainText = $"{agentId}{timestamp}{md5key}";
            var key = GetHashMD5(plainText);

            var apiLYRequest = new LYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<LYGameRecordResponse>(apiRecordUrl, action, apiLYRequest, "GameRecord");
        }

        public ValueTask<LYApiResponse<LYGameUrlResponse>> GetGameUrlAsync(Uri apiUrl, string deskey, string md5key, string agentId, DateTimeOffset dateTimeOffset, LYGameUrlRequest request)
        {
            var action = LYAction.GetGameUrl;
            request.Action = GetAction(action);

            var timestamp = dateTimeOffset.ToUnixTimeMilliseconds();
            var dataPath = GetPath(request);
            var param = GetAesEncrypt(dataPath, deskey);
            var plainText = $"{agentId}{timestamp}{md5key}";
            var key = GetHashMD5(plainText);

            var apiLYRequest = new LYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<LYGameUrlResponse>(apiUrl, action, apiLYRequest, "GameUrl");
        }

        public ValueTask<LYApiResponse<LYKickResponse>> KickAsync(Uri apiUrl, string deskey, string md5key, string agentId, LYKickRequest request)
        {
            var action = LYAction.Kick;
            request.Action = GetAction(action);

            var timestamp = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeMilliseconds();
            var dataPath = GetPath(request);
            var param = GetAesEncrypt(dataPath, deskey);
            var plainText = $"{agentId}{timestamp}{md5key}";
            var key = GetHashMD5(plainText);

            var apiLYRequest = new LYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<LYKickResponse>(apiUrl, action, apiLYRequest, "Kick");
        }

        private async ValueTask<LYApiResponse<TData>> GetAPIAsync<TData>(Uri apiUrl, LYAction action, LYApiRequest request, string logPrefix)
        {
            logPrefix = $"{nameof(LYApiClient).ToLower() }.{logPrefix}";
            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(LYApiResponse<TData>);

            var paths = GetPath(request);
            var url = $"{apiUrl?.ToString()}?{paths}";

            var httpResponseMessage = await _client.GetAsync(url);
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
                RequestContent = JsonSerializer.Serialize(request),
                ResponseHeaders = httpResponseMessage.RequestMessage.Headers.ToString(),
                ResponseContent = responseMessage,
                ResponseCode = (short)httpResponseMessage.StatusCode,
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                RecordedOn = DateTimeOffset.UtcNow
            });

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                response = JsonSerializer.Deserialize<LYApiResponse<TData>>(responseMessage);
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

            return response;
        }

        private string GetAesEncrypt(string data, string key)
        {
            if (string.IsNullOrEmpty(data))
                return null;

            var dataByte = Encoding.UTF8.GetBytes(data);

            using (var aes = Aes.Create())
            {
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.GenerateIV();
                using (var cryptoTransform = aes.CreateEncryptor())
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(dataByte, 0, dataByte.Length);
                            cryptoStream.FlushFinalBlock();
                            var encrypted = memoryStream.ToArray();
                            var base64Str = Convert.ToBase64String(encrypted, 0, encrypted.Length);
                            return System.Web.HttpUtility.UrlEncode(base64Str);
                        }
                    }
                }
            }
        }

        private int GetAction(LYAction action)
        {
            return LYActionMapper.Mapper.Where(x => x.Key == action).First().Value;
        }

        private string GetPath<TRequest>(TRequest request, JsonSerializerOptions options = null)
        {
            var value = JsonSerializer.Serialize(request, options);
            var items = JsonSerializer.Deserialize<Dictionary<string, object>>(value, options);
            return string.Join("&", items.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
        }

        private string GetHashMD5(string data)
        {
            using (var md5 = MD5.Create())
            {
                byte[] dataByte = Encoding.UTF8.GetBytes(data);
                byte[] hash = md5.ComputeHash(dataByte);
                string result = String.Empty;
                for (int i = 0; i < hash.Length; i++)
                    result += hash[i].ToString("x").PadLeft(2, '0');
                return result;
            }
        }
    }
}
