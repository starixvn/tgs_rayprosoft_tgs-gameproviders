using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.WGSports.Models;
using TGS.GameProviders.ApiClients.WGSports.Models.Commons;
using TGS.GameProviders.Domains.Extensions;

namespace TGS.GameProviders.ApiClients.WGSports
{
    internal class WGSportsApiClient : IWGSportsApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<WGSportsApiClient> _logger;
        private readonly ITgsLog _tgsLog;
        private readonly IWGSportsUtility _utility;

        public WGSportsApiClient(
            HttpClient client,
            ILogger<WGSportsApiClient> logger,
            ITgsLog tgsLog,
            IWGSportsUtility utility)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
        }

        public ValueTask<WGSportsResponse> AccessMoneyAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, WGSportsAccessMoneyRequest source)
        {
            var data = $"{source.PlayerCode}|{(int)source.AmountType}|{source.OrderId}|{source.Amount}";
            var dataEncrypt = _utility.AesEncrypt(data, partnerKey, partnerIV);
            var url = new Uri($"{apiUrl}?m=api&c=index&a=AccessMoney&Hash={dataEncrypt}&PartnerCode={partnerCode}");

            return GetAsync<WGSportsAccessMoneyRequest, WGSportsResponse>(url, source, "AccessMoney");
        }

        public ValueTask<WGSportsResponse> AccessMoneyQueryAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, WGSportsAccessMoneyQueryRequest source)
        {
            var data = source.OrderId;
            var dataEncrypt = _utility.AesEncrypt(data, partnerKey, partnerIV);
            var url = new Uri($"{apiUrl}?m=api&c=index&a=AccessMoneyQuery&Hash={dataEncrypt}&PartnerCode={partnerCode}");

            return GetAsync<WGSportsAccessMoneyQueryRequest, WGSportsResponse>(url, source, "AccessMoneyQuery");
        }

        public ValueTask<WGSportsGetBalanceResponse> GetBalanceAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, WGSportsGetBalanceRequest source)
        {
            var data = source.PlayerCode;
            var dataEncrypt = _utility.AesEncrypt(data, partnerKey, partnerIV);
            var url = new Uri($"{apiUrl}?m=api&c=index&a=GetBalance&Hash={dataEncrypt}&PartnerCode={partnerCode}");

            return GetAsync<WGSportsGetBalanceRequest, WGSportsGetBalanceResponse>(url, source, "Balance");
        }

        public ValueTask<WGSportsGameRecordResponse> GetGameRecordAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, WGSportsGameRecordRequest source)
        {
            var data = $"|{source.GameRecordTime.Description()}|{source.StartTime:yyyy-MM-dd HH:mm:ss}|{source.EndTime:yyyy-MM-dd HH:mm:ss}";
            var dataEncrypt = _utility.AesEncrypt(data, partnerKey, partnerIV);
            var url = new Uri($"{apiUrl}?m=api&c=index&a=ShowBetList&Hash={dataEncrypt}&PartnerCode={partnerCode}");

            return GetAsync<WGSportsGameRecordRequest, WGSportsGameRecordResponse>(url, source, "GameRecord");
        }

        public ValueTask<WGSportsLoginResponse> GetLoginAsync(Uri apiUrl, string partnerCode, WGSportsLoginRequest source)
        {
            var url = new Uri($"{apiUrl}?m=api&c=index&a=Login&Token={System.Web.HttpUtility.UrlEncode(source.Token)}&PartnerCode={partnerCode}&Lang={source.Lang}");

            return GetAsync<WGSportsLoginRequest, WGSportsLoginResponse>(url, source, "Login");
        }

        public ValueTask<WGSportsPrepareLoginResponse> GetPrepareLoginAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, WGSportsPrepareLoginRequest source)
        {
            var data = $"{source.Timestamp}|{source.PlayerCode}|{source.PlayerNickname}|{source.CurrencyCode}|{source.Handicap}";
            var dataEncrypt = _utility.AesEncrypt(data, partnerKey, partnerIV);
            var url = new Uri($"{apiUrl}?m=api&c=index&a=PrepareLogin&Hash={dataEncrypt}&PartnerCode={partnerCode}");

            return GetAsync<WGSportsPrepareLoginRequest, WGSportsPrepareLoginResponse>(url, source, "PrepareLogin");
        }

        private async ValueTask<TResult> GetAsync<TSource, TResult>(Uri url, TSource request, string logPrefix)
        {
            logPrefix = $"{nameof(WGSportsApiClient).ToLower()}.{logPrefix}";
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
