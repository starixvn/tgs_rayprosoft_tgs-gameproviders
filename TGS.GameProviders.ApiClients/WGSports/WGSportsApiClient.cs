using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.WGSports.Models;
using TGS.GameProviders.ApiClients.WGSports.Models.Commons;
using TGS.GameProviders.Domains.Extensions;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.ApiClients.WGSports
{
    internal sealed class WGSportsApiClient : IWGSportsApiClient
    {
        private readonly HttpClient _client;
        private readonly IWGSportsUtility _utility;

        public WGSportsApiClient(
            HttpClient client,
            IWGSportsUtility utility)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
        }

        public ValueTask<WGSportsResponse> AccessMoneyAsync(WGSportsConfiguration configuration, WGSportsAccessMoneyRequest source)
        {
            var data = $"{source.PlayerCode}|{(int)source.AmountType}|{source.OrderId}|{source.Amount}";
            var dataEncrypt = _utility.AesEncrypt(data, configuration.PartnerKey, configuration.IV);
            var url = new Uri($"{configuration.ApiUrl}?m=api&c=index&a=AccessMoney&Hash={dataEncrypt}&PartnerCode={configuration.PartnerCode}");

            return GetAPIAsync<WGSportsAccessMoneyRequest, WGSportsResponse>(url, source);
        }

        public ValueTask<WGSportsResponse> AccessMoneyQueryAsync(WGSportsConfiguration configuration, WGSportsAccessMoneyQueryRequest source)
        {
            var data = source.OrderId;
            var dataEncrypt = _utility.AesEncrypt(data, configuration.PartnerKey, configuration.IV);
            var url = new Uri($"{configuration.ApiUrl}?m=api&c=index&a=AccessMoneyQuery&Hash={dataEncrypt}&PartnerCode={configuration.PartnerCode}");

            return GetAPIAsync<WGSportsAccessMoneyQueryRequest, WGSportsResponse>(url, source);
        }

        public ValueTask<WGSportsGetBalanceResponse> GetBalanceAsync(WGSportsConfiguration configuration, WGSportsGetBalanceRequest source)
        {
            var data = source.PlayerCode;
            var dataEncrypt = _utility.AesEncrypt(data, configuration.PartnerKey, configuration.IV);
            var url = new Uri($"{configuration.ApiUrl}?m=api&c=index&a=GetBalance&Hash={dataEncrypt}&PartnerCode={configuration.PartnerCode}");

            return GetAPIAsync<WGSportsGetBalanceRequest, WGSportsGetBalanceResponse>(url, source);
        }

        public ValueTask<WGSportsGameRecordResponse> GetGameRecordAsync(WGSportsConfiguration configuration, WGSportsGameRecordRequest source)
        {
            var data = $"|{source.GameRecordTime.Description()}|{source.StartTime:yyyy-MM-dd HH:mm:ss}|{source.EndTime:yyyy-MM-dd HH:mm:ss}";
            var dataEncrypt = _utility.AesEncrypt(data, configuration.PartnerKey, configuration.IV);
            var url = new Uri($"{configuration.ApiUrl}?m=api&c=index&a=ShowBetList&Hash={dataEncrypt}&PartnerCode={configuration.PartnerCode}");

            return GetAPIAsync<WGSportsGameRecordRequest, WGSportsGameRecordResponse>(url, source);
        }

        public ValueTask<WGSportsLoginResponse> GetLoginAsync(WGSportsConfiguration configuration, WGSportsLoginRequest source)
        {
            var url = new Uri($"{configuration.ApiUrl}?m=api&c=index&a=Login&Token={System.Web.HttpUtility.UrlEncode(source.Token)}&PartnerCode={configuration.PartnerCode}&Lang={source.Lang}");

            return GetAPIAsync<WGSportsLoginRequest, WGSportsLoginResponse>(url, source);
        }

        public ValueTask<WGSportsPrepareLoginResponse> GetPrepareLoginAsync(WGSportsConfiguration configuration, WGSportsPrepareLoginRequest source)
        {
            var data = $"{source.Timestamp}|{source.PlayerCode}|{source.PlayerNickname}|{source.CurrencyCode}|{source.Handicap}";
            var dataEncrypt = _utility.AesEncrypt(data, configuration.PartnerKey, configuration.IV);
            var url = new Uri($"{configuration.ApiUrl}?m=api&c=index&a=PrepareLogin&Hash={dataEncrypt}&PartnerCode={configuration.PartnerCode}");

            return GetAPIAsync<WGSportsPrepareLoginRequest, WGSportsPrepareLoginResponse>(url, source);
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
