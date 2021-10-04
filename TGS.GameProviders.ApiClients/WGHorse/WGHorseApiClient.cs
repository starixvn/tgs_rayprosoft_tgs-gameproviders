using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.WGHorse.Models;
using TGS.GameProviders.ApiClients.WGHorse.Models.Commons;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.ApiClients.WGHorse
{
    internal class WGHorseApiClient : IWGHorseApiClient
    {
        private readonly HttpClient _client;
        private readonly IWGHorseUtility _utility;

        public WGHorseApiClient(
            HttpClient client,
            IWGHorseUtility utility)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
        }

        public ValueTask<WGHorseResponse> AccessMoneyAsync(WGHorseConfiguration configuration, WGHorseAccessMoneyRequest source)
        {
            var data = $"{source.PlayerCode}|{ (int)source.AmountType}|{source.OrderId}|{source.Amount}";
            var dataEncrypt = _utility.AesEncrypt(data, configuration.PartnerKey, configuration.IV);
            var url = new Uri($"{configuration.ApiUrl}?m=api&c=index&a=AccessMoney&Hash={dataEncrypt}&PartnerCode={configuration.PartnerCode}");

            return GetAPIAsync<WGHorseAccessMoneyRequest, WGHorseResponse>(url, source);
        }

        public ValueTask<WGHorseResponse> AccessMoneyQueryAsync(WGHorseConfiguration configuration, WGHorseAccessMoneyQueryRequest source)
        {
            var data = source.OrderId;
            var dataEncrypt = _utility.AesEncrypt(data, configuration.PartnerKey, configuration.IV);
            var url = new Uri($"{configuration.ApiUrl}?m=api&c=index&a=AccessMoneyQuery&Hash={dataEncrypt}&PartnerCode={configuration.PartnerCode}");

            return GetAPIAsync<WGHorseAccessMoneyQueryRequest, WGHorseResponse>(url, source);
        }

        public ValueTask<WGHorseGetBalanceResponse> GetBalanceAsync(WGHorseConfiguration configuration, WGHorseGetBalanceRequest source)
        {
            var data = source.PlayerCode;
            var dataEncrypt = _utility.AesEncrypt(data, configuration.PartnerKey, configuration.IV);
            var url = new Uri($"{configuration.ApiUrl}?m=api&c=index&a=GetBalance&Hash={dataEncrypt}&PartnerCode={configuration.PartnerCode}");

            return GetAPIAsync<WGHorseGetBalanceRequest, WGHorseGetBalanceResponse>(url, source);
        }

        public ValueTask<WGHorseGameRecordResponse> GetGameRecordAsync(WGHorseConfiguration configuration, WGHorseGameRecordRequest source)
        {
            var data = $"|{source.StartTime:yyyy-MM-dd}|{source.EndTime:yyyy-MM-dd}|{source.PageIndex}|{source.PageSize}";
            var dataEncrypt = _utility.AesEncrypt(data, configuration.PartnerKey, configuration.IV);
            var url = new Uri($"{configuration.ApiUrl}?m=api&c=index&a=ShowBetList&Hash={dataEncrypt}&PartnerCode={configuration.PartnerCode}");

            return GetAPIAsync<WGHorseGameRecordRequest, WGHorseGameRecordResponse>(url, source);
        }

        public ValueTask<WGHorseLoginResponse> GetLoginAsync(WGHorseConfiguration configuration, WGHorseLoginRequest source)
        {
            var url = new Uri($"{configuration.ApiUrl}?m=api&c=index&a=Login&Token={System.Web.HttpUtility.UrlEncode(source.Token)}&PartnerCode={configuration.PartnerCode}&Lang={source.Lang}");

            return GetAPIAsync<WGHorseLoginRequest, WGHorseLoginResponse>(url, source);
        }

        public ValueTask<WGHorsePrepareLoginResponse> GetPrepareLoginAsync(WGHorseConfiguration configuration, WGHorsePrepareLoginRequest source)
        {
            var data = $"{source.Timestamp}|{source.PlayerCode}|{source.PlayerNickname}|{source.CurrencyCode}";
            var dataEncrypt = _utility.AesEncrypt(data, configuration.PartnerKey, configuration.IV);
            var url = new Uri($"{configuration.ApiUrl}?m=api&c=index&a=PrepareLogin&Hash={dataEncrypt}&PartnerCode={configuration.PartnerCode}");

            return GetAPIAsync<WGHorsePrepareLoginRequest, WGHorsePrepareLoginResponse>(url, source);
        }

        private async ValueTask<ResultData> GetAPIAsync<SourceData, ResultData>(Uri url, SourceData request)
        {
            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(ResultData);

            var httpResponseMessage = await _client.GetAsync(url);
            sw.Stop();

            using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result))
            {
                responseMessage = await sr.ReadToEndAsync() ?? string.Empty;
            }

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                response = JsonSerializer.Deserialize<ResultData>(responseMessage);
            }

            return response;
        }
    }
}
