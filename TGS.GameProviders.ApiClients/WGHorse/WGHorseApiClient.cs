using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.WGHorse.Models;
using TGS.GameProviders.ApiClients.WGHorse.Models.Commons;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.Extensions;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.ApiClients;

namespace TGS.GameProviders.ApiClients.WGHorse
{
    internal class WGHorseApiClient : IWGHorseApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<WGHorseApiClient> _logger;
        private readonly ITgsLog _tgsLog;
        private readonly IWGHorseUtility _utility;

        public WGHorseApiClient(
            HttpClient client,
            ILogger<WGHorseApiClient> logger,
            ITgsLog tgsLog,
            IWGHorseUtility utility)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
        }

        public ValueTask<WGHorseResponse> AccessMoneyAsync(WGHorseConfiguration configuration, WGHorseAccessMoneyRequest source)
        {
            var data = $"{source.PlayerCode}|{ (int)source.AmountType}|{source.OrderId}|{source.Amount}";
            var dataEncrypt = _utility.AesEncrypt(data, configuration.PartnerKey, configuration.IV);
            var url = new Uri($"{configuration.ApiUrl}?m=api&c=index&a=AccessMoney&Hash={dataEncrypt}&PartnerCode={configuration.PartnerCode}");

            return GetAPIAsync<WGHorseAccessMoneyRequest, WGHorseResponse>(url, source, "AccessMoney");
        }

        public ValueTask<WGHorseResponse> AccessMoneyQueryAsync(WGHorseConfiguration configuration, WGHorseAccessMoneyQueryRequest source)
        {
            var data = source.OrderId;
            var dataEncrypt = _utility.AesEncrypt(data, configuration.PartnerKey, configuration.IV);
            var url = new Uri($"{configuration.ApiUrl}?m=api&c=index&a=AccessMoneyQuery&Hash={dataEncrypt}&PartnerCode={configuration.PartnerCode}");

            return GetAPIAsync<WGHorseAccessMoneyQueryRequest, WGHorseResponse>(url, source, "AccessMoneyQuery");
        }

        public ValueTask<WGHorseGetBalanceResponse> GetBalanceAsync(WGHorseConfiguration configuration, WGHorseGetBalanceRequest source)
        {
            var data = source.PlayerCode;
            var dataEncrypt = _utility.AesEncrypt(data, configuration.PartnerKey, configuration.IV);
            var url = new Uri($"{configuration.ApiUrl}?m=api&c=index&a=GetBalance&Hash={dataEncrypt}&PartnerCode={configuration.PartnerCode}");

            return GetAPIAsync<WGHorseGetBalanceRequest, WGHorseGetBalanceResponse>(url, source, "Balance");
        }

        public ValueTask<WGHorseGameRecordResponse> GetGameRecordAsync(WGHorseConfiguration configuration, WGHorseGameRecordRequest source)
        {
            var data = $"|{source.StartTime:yyyy-MM-dd}|{source.EndTime:yyyy-MM-dd}|{source.PageIndex}|{source.PageSize}";
            var dataEncrypt = _utility.AesEncrypt(data, configuration.PartnerKey, configuration.IV);
            var url = new Uri($"{configuration.ApiUrl}?m=api&c=index&a=ShowBetList&Hash={dataEncrypt}&PartnerCode={configuration.PartnerCode}");

            return GetAPIAsync<WGHorseGameRecordRequest, WGHorseGameRecordResponse>(url, source, "GameRecord");
        }

        public ValueTask<WGHorseLoginResponse> GetLoginAsync(WGHorseConfiguration configuration, WGHorseLoginRequest source)
        {
            var url = new Uri($"{configuration.ApiUrl}?m=api&c=index&a=Login&Token={System.Web.HttpUtility.UrlEncode(source.Token)}&PartnerCode={configuration.PartnerCode}&Lang={source.Lang}");

            return GetAPIAsync<WGHorseLoginRequest, WGHorseLoginResponse>(url, source, "Login");
        }

        public ValueTask<WGHorsePrepareLoginResponse> GetPrepareLoginAsync(WGHorseConfiguration configuration, WGHorsePrepareLoginRequest source)
        {
            var data = $"{source.Timestamp}|{source.PlayerCode}|{source.PlayerNickname}|{source.CurrencyCode}";
            var dataEncrypt = _utility.AesEncrypt(data, configuration.PartnerKey, configuration.IV);
            var url = new Uri($"{configuration.ApiUrl}?m=api&c=index&a=PrepareLogin&Hash={dataEncrypt}&PartnerCode={configuration.PartnerCode}");

            return GetAPIAsync<WGHorsePrepareLoginRequest, WGHorsePrepareLoginResponse>(url, source, "PrepareLogin");
        }

        private async ValueTask<TResult> GetAPIAsync<TSource, TResult>(Uri url, TSource request, string logPrefix)
        {
            logPrefix = $"{GameProviderType.WGHorse}.{logPrefix}";
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
