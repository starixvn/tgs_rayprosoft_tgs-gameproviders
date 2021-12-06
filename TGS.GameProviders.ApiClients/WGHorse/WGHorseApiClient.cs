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
using TGS.GameProviders.Core.WGHorse;
using TGS.GameProviders.Core.WGHorse.Models;
using TGS.GameProviders.Core.WGHorse.Models.Commons;

namespace TGS.GameProviders.ApiClients.WGHorse
{
    internal class WGHorseApiClient : IWGHorseApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<WGHorseApiClient> _logger;
        private readonly ITgsLog _tgsLog;

        public WGHorseApiClient(
            HttpClient client,
            ILogger<WGHorseApiClient> logger,
            ITgsLog tgsLog)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
        }

        public async ValueTask<bool> AccessMoneyFundInAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, string playerCode, string orderId, decimal amount)
        {
            var request = new
            {
                PlayerCode = playerCode,
                AmountType = WGHorseAmountTypes.FundIn,
                OrderId = orderId,
                Amount = amount
            };
            var data = $"{request.PlayerCode}|{(int)request.AmountType}|{request.OrderId}|{request.Amount}";
            var dataEncrypt = GetAesEncrypt(data, partnerKey, partnerIV);
            var url = new Uri($"{apiUrl}?m=api&c=index&a=AccessMoney&Hash={dataEncrypt}&PartnerCode={partnerCode}");

            var response = await GetAsync<WGHorseResponse>(
                url,
                request,
                "AccessMoneyFundIn").ConfigureAwait(false);
            return (response != null && response?.ReturnCode == 1);
        }

        public async ValueTask<bool> AccessMoneyFundOutAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, string playerCode, string orderId, decimal amount)
        {
            var request = new
            {
                PlayerCode = playerCode,
                AmountType = WGHorseAmountTypes.FundOut,
                OrderId = orderId,
                Amount = amount
            };
            var data = $"{request.PlayerCode}|{(int)request.AmountType}|{request.OrderId}|{request.Amount}";
            var dataEncrypt = GetAesEncrypt(data, partnerKey, partnerIV);
            var url = new Uri($"{apiUrl}?m=api&c=index&a=AccessMoney&Hash={dataEncrypt}&PartnerCode={partnerCode}");

            var response = await GetAsync<WGHorseResponse>(
                url,
                request,
                "AccessMoneyFundOut").ConfigureAwait(false);
            return (response != null && response?.ReturnCode == 1);
        }

        public async ValueTask<bool> AccessMoneyQueryAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, string orderId)
        {
            var request = new
            {
                OrderId = orderId
            };
            var data = orderId;
            var dataEncrypt = GetAesEncrypt(data, partnerKey, partnerIV);
            var url = new Uri($"{apiUrl}?m=api&c=index&a=AccessMoneyQuery&Hash={dataEncrypt}&PartnerCode={partnerCode}");

            var response = await GetAsync<WGHorseResponse>(
                url,
                request,
                "AccessMoneyQuery").ConfigureAwait(false);

            return (response != null && response?.ReturnCode == 1);
        }

        public async ValueTask<(bool IsPlayer, decimal? Balance)> GetBalanceAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, string playerCode)
        {
            var request = new
            {
                PlayerCode = playerCode
            };
            var data = playerCode;
            var dataEncrypt = GetAesEncrypt(data, partnerKey, partnerIV);
            var url = new Uri($"{apiUrl}?m=api&c=index&a=GetBalance&Hash={dataEncrypt}&PartnerCode={partnerCode}");

            var response = await GetAsync<WGHorseGetBalanceResponse>(url, request, "Balance").ConfigureAwait(false);

            if (response.ReturnCode == 602)
            {
                return (false, null);
            }

            return (true, response.ReturnCode == 1 ? decimal.Parse(response.Balance) : null);
        }

        public async ValueTask<(bool Success, WGHorseGameRecord[]? GameRecord)> GetGameRecordAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, DateTimeOffset startTime, DateTimeOffset endTime, int pageIndex, int pageSize)
        {
            var request = new
            {
                StartTime = startTime,
                EndTime = endTime,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = $"|{startTime:yyyy-MM-dd}|{endTime:yyyy-MM-dd}|{pageIndex}|{pageSize}";
            var dataEncrypt = GetAesEncrypt(data, partnerKey, partnerIV);
            var url = new Uri($"{apiUrl}?m=api&c=index&a=ShowBetList&Hash={dataEncrypt}&PartnerCode={partnerCode}");

            var response = await GetAsync<WGHorseGameRecordResponse>(url, request, "GameRecord").ConfigureAwait(false);
            if (response == null)
            {
                return (false, null);
            }

            if (response?.ReturnCode != 1)
            {
                if (response?.ReturnCode != 503)
                {
                    return new() { Success = false };
                }
            }

            if (pageIndex.ToString().IgnoreCaseNotEquals(response.Page)
               || response.ArrayInvoice == null || response.ArrayInvoice.Length == 0)
            {
                return (true, null);
            }
            return (true, response.ArrayInvoice);
        }

        public async ValueTask<Uri?> GetLoginAsync(Uri apiUrl, string partnerCode, string token, string lang)
        {
            var request = new
            {
                Token = token,
                Lang = lang
            };

            var url = new Uri($"{apiUrl}?m=api&c=index&a=Login&Token={System.Web.HttpUtility.UrlEncode(token)}&PartnerCode={partnerCode}&Lang={lang}");

            var response = await GetAsync<WGHorseLoginResponse>(url, request, "Login").ConfigureAwait(false);

            return response.ReturnCode is not 1 ? null : new(response.LoginUrl);
        }

        public async ValueTask<string?> GetPrepareLoginAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, long timestamp, string playerCode, string playerNickname, string currencyCode)
        {
            var request = new
            {
                Timestamp = timestamp,
                PlayerCode = playerCode,
                PlayerNickname = playerNickname,
                CurrencyCode = currencyCode,
            };
            var data = $"{timestamp}|{playerCode}|{playerNickname}|{currencyCode}";
            var dataEncrypt = GetAesEncrypt(data, partnerKey, partnerIV);
            var url = new Uri($"{apiUrl}?m=api&c=index&a=PrepareLogin&Hash={dataEncrypt}&PartnerCode={partnerCode}");

            var response = await GetAsync<WGHorsePrepareLoginResponse>(url, request, "PrepareLogin").ConfigureAwait(false);

            return (response.ReturnCode is not 1) ? null : response.Token;
        }

        public async ValueTask LogoutAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, string playerCode)
        {
            var request = new
            {
                PlayerCode = playerCode
            };
            var data = $"{playerCode}";
            var dataEncrypt = GetAesEncrypt(data, partnerKey, partnerIV);
            var url = new Uri($"{apiUrl}?m=api&c=index&a=Logout&Hash={dataEncrypt}&PartnerCode={partnerCode}");

            await GetAsync<WGHorseResponse>(url, request, "Logout").ConfigureAwait(false);
        }

        private string GetAesEncrypt(string data, string secretKey, string secretIV)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] ivBytes = Encoding.UTF8.GetBytes(secretIV);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            using (var aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Key = keyBytes;
                aes.IV = ivBytes;

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
                            return System.Web.HttpUtility.UrlEncode(base64Str);
                        }
                    }
                }
            }
        }

        private async ValueTask<TResult> GetAsync<TResult>(Uri url, object request, string logPrefix)
        {
            logPrefix = $"{nameof(WGHorseApiClient).ToLower()}.{logPrefix}";
            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(TResult);

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
                response = JsonSerializer.Deserialize<TResult>(responseMessage);
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
    }
}
