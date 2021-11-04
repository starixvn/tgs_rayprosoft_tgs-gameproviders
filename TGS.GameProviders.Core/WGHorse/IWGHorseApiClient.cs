using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.WGHorse.Models;
using TGS.GameProviders.Core.WGHorse.Models.Commons;

namespace TGS.GameProviders.Core.WGHorse
{
    public interface IWGHorseApiClient
    {
        ValueTask<WGHorseResponse> AccessMoneyAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, WGHorseAccessMoneyRequest source);

        ValueTask<WGHorseResponse> AccessMoneyQueryAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, WGHorseAccessMoneyQueryRequest source);

        ValueTask<WGHorseGetBalanceResponse> GetBalanceAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, WGHorseGetBalanceRequest source);

        ValueTask<WGHorseGameRecordResponse> GetGameRecordAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, WGHorseGameRecordRequest source);

        ValueTask<WGHorseLoginResponse> GetLoginAsync(Uri apiUrl, string partnerCode, WGHorseLoginRequest source);

        ValueTask<WGHorsePrepareLoginResponse> GetPrepareLoginAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, WGHorsePrepareLoginRequest source);

        ValueTask<WGHorseResponse> LogoutAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, string playerCode);
    }
}
