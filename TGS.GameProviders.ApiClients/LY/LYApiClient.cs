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
using TGS.GameProviders.ApiClients.LY.Models;
using TGS.GameProviders.ApiClients.LY.Models.Commons;
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Core.LY;
using TGS.GameProviders.Core.LY.Models;

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

        public async ValueTask<bool> FundCheckingAsync(Uri apiUrl, string deskey, string md5key, string agentId, string orderId)
        {
            var request = new Dictionary<string, object>() {
                {"s", GetAction(LYAction.FundChecking)},
                {"orderid",orderId}
            };

            var timestamp = GetUnixTimeMilliseconds(GetDateTimeOffsetNowTimeZone8());
            var dataPath = GetPath(request);
            var param = GetAesEncrypt(dataPath, deskey);
            var plainText = $"{agentId}{timestamp}{md5key}";
            var key = GetHashMD5(plainText);

            var apiKYRequest = new LYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            var response = await GetAPIAsync<LYFundCheckingResponse>(
                apiUrl,
                apiKYRequest,
                LYAction.FundChecking.ToString()).ConfigureAwait(false);

            return (response != null && response?.Data.Code == 0 && response?.Data.Status == 0);
        }

        public async ValueTask<(bool Success, string orderId)> FundInAsync(Uri apiUrl, string deskey, string md5key, string agentId, string playerCode, decimal amount)
        {
            var dateTimeOffset = GetDateTimeOffsetNowTimeZone8();
            var orderId = $"{agentId}{dateTimeOffset:yyyyMMddHHmmssfff}{playerCode}";
            var request = new Dictionary<string, object>() {
                {"s", GetAction(LYAction.FundIn)}  ,
                {"account",playerCode},
                {"orderid",orderId},
                {"money",amount}
            };

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

            var response = await GetAPIAsync<LYFnudResponse>(
                apiUrl,
                apiLYRequest,
                LYAction.FundIn.ToString()).ConfigureAwait(false);

            return (response != null && response?.Data.Code == 0) ?
                new(true, orderId) :
                new(false, orderId);
        }

        public async ValueTask<(bool Success, string orderId)> FundOutAsync(Uri apiUrl, string deskey, string md5key, string agentId, string playerCode, decimal amount)
        {
            var dateTimeOffset = GetDateTimeOffsetNowTimeZone8();
            var orderId = $"{agentId}{dateTimeOffset:yyyyMMddHHmmssfff}{playerCode}";
            var request = new Dictionary<string, object>() {
                {"s", GetAction(LYAction.FundOut)}  ,
                {"account",playerCode},
                {"orderid",orderId},
                {"money",amount}
            };

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

            var response = await GetAPIAsync<LYFnudResponse>(
                apiUrl,
                apiLYRequest,
                LYAction.FundOut.ToString()).ConfigureAwait(false);

            return (response != null && response?.Data.Code == 0) ?
                new(true, orderId) :
                new(false, orderId);
        }

        public async ValueTask<decimal?> GetBalanceAsync(Uri apiUrl, string deskey, string md5key, string agentId, string playerCode)
        {
            var request = new Dictionary<string, object>() {
                {"s", GetAction(LYAction.GetBalance)},
                {"account",playerCode}
            };

            var timestamp = GetUnixTimeMilliseconds(GetDateTimeOffsetNowTimeZone8());
            var dataPath = GetPath(request);
            var param = GetAesEncrypt(dataPath, deskey);
            var plainText = $"{agentId}{timestamp}{md5key}";
            var key = GetHashMD5(plainText);

            var apiKYRequest = new LYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            var response = await GetAPIAsync<LYGetBalanceResponse>(
                apiUrl,
                apiKYRequest,
                LYAction.GetBalance.ToString()).ConfigureAwait(false);

            return (response?.Data.Code == 0) ? response.Data.Balance : null;
        }

        public async ValueTask<(bool Success, LYGameRecordList GameRecordList)> GetGameRecordAsync(Uri apiRecordUrl, string deskey, string md5key, string agentId, long startTime, long endTime)
        {
            var request = new Dictionary<string, object>() {
                {"s", GetAction(LYAction.GameRecord)},
                {"startTime",startTime},
                {"endTime",endTime}
            };

            var timestamp = GetUnixTimeMilliseconds(GetDateTimeOffsetNowTimeZone8());
            var dataPath = GetPath(request);
            var param = GetAesEncrypt(dataPath, deskey);
            var plainText = $"{agentId}{timestamp}{md5key}";
            var key = GetHashMD5(plainText);

            var apiKYRequest = new LYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            var response = await GetAPIAsync<LYGameRecordResponse>(
                apiRecordUrl,
                apiKYRequest,
                LYAction.GameRecord.ToString()).ConfigureAwait(false);

            if (response == null || response?.Data.Code != 0)
            {
                if (response?.Data.Code == 16)
                {
                    return new(true, null);
                }
                else
                {
                    return new(false, null);
                }
            }

            if (response?.Data == null || response?.Data.List.GameID.Length == 0)
            {
                return new(true, null);
            }

            return new(true, response.Data.List);
        }

        public async ValueTask<Uri> GetGameUrlAsync(Uri apiUrl, string deskey, string md5key, string agentId, string playerCode, string ipAddress, string brandCode, int gameExternalID)
        {
            var dateTimeOffset = GetDateTimeOffsetNowTimeZone8();
            var request = new Dictionary<string, object>() {
                {"s", GetAction(LYAction.GetGameUrl)},
                {"account",playerCode},
                {"money",0m},
                {"orderid",$"{agentId}{dateTimeOffset:yyyyMMddHHmmssfff}{playerCode}"},
                {"ip",ipAddress},
                {"lineCode",brandCode},
                {"KindID",gameExternalID},
            };

            var timestamp = GetUnixTimeMilliseconds(dateTimeOffset);
            var dataPath = GetPath(request);
            var param = GetAesEncrypt(dataPath, deskey);
            var plainText = $"{agentId}{timestamp}{md5key}";
            var key = GetHashMD5(plainText);

            var apiKYRequest = new LYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            var response = await GetAPIAsync<LYGameUrlResponse>(
                apiUrl,
                apiKYRequest,
                LYAction.GetGameUrl.ToString()).ConfigureAwait(false);

            return (response != null && response?.Data.Code == 0) ? new(response.Data.Url) : null;
        }

        public async ValueTask KickAsync(Uri apiUrl, string deskey, string md5key, string agentId, string playerCode)
        {
            var request = new Dictionary<string, object>() {
                {"s", GetAction(LYAction.Kick)},
                {"account",playerCode}
            };

            var timestamp = GetUnixTimeMilliseconds(GetDateTimeOffsetNowTimeZone8());
            var dataPath = GetPath(request);
            var param = GetAesEncrypt(dataPath, deskey);
            var plainText = $"{agentId}{timestamp}{md5key}";
            var key = GetHashMD5(plainText);

            var apiKYRequest = new LYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            await GetAPIAsync<LYBaseDataResponse>(
                apiUrl,
                apiKYRequest,
                LYAction.Kick.ToString()).ConfigureAwait(false);
        }

        private static string GetHashMD5(string data)
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

        private static string GetPath(Dictionary<string, object> request)
            => string.Join("&", request.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

        private static string GetPath(LYApiRequest request, JsonSerializerOptions options = null)
        {
            var value = JsonSerializer.Serialize(request, options);
            var items = JsonSerializer.Deserialize<Dictionary<string, object>>(value, options);
            return string.Join("&", items.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
        }

        private static long GetUnixTimeMilliseconds(DateTimeOffset dateTimeOffset)
            => dateTimeOffset.ToUnixTimeMilliseconds();

        private int GetAction(LYAction action)
        {
            return LYActionMapper.Mapper.Where(x => x.Key == action).First().Value;
        }

        private string GetAesEncrypt(string data, string secretKey)
        {
            if (string.IsNullOrEmpty(data))
                return null;

            var dataByte = Encoding.UTF8.GetBytes(data);

            using (var aes = Aes.Create())
            {
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = Encoding.UTF8.GetBytes(secretKey);
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

        private async ValueTask<LYApiResponse<TData>> GetAPIAsync<TData>(Uri apiUrl, LYApiRequest request, string logPrefix)
        {
            logPrefix = $"{nameof(LYApiClient).ToLower() }.{logPrefix}";

            var sw = Stopwatch.StartNew();

            var paths = GetPath(request);

            var url = $"{apiUrl?.ToString()}?{paths}";

            var httpResponseMessage = await _client.GetAsync(url);

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
                RequestContent = JsonSerializer.Serialize(request).NoLongerThan(4096),
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
                return JsonSerializer.Deserialize<LYApiResponse<TData>>(responseMessage);
            }

            return default;
        }

        private DateTimeOffset GetDateTimeOffsetNowTimeZone8()
            => DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
    }
}
