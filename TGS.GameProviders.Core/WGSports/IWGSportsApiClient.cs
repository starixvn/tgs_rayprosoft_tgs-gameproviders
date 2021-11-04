using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.WGSports.Models;
using TGS.GameProviders.Core.WGSports.Models.Commons;

namespace TGS.GameProviders.Core.WGSports
{
    public interface IWGSportsApiClient
    {
        ValueTask<WGSportsResponse> AccessMoneyAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, WGSportsAccessMoneyRequest source);

        ValueTask<WGSportsResponse> AccessMoneyQueryAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, WGSportsAccessMoneyQueryRequest source);

        ValueTask<WGSportsGetBalanceResponse> GetBalanceAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, WGSportsGetBalanceRequest source);

        ValueTask<WGSportsGameRecordResponse> GetGameRecordAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, WGSportsGameRecordRequest source);

        ValueTask<WGSportsLoginResponse> GetLoginAsync(Uri apiUrl, string partnerCode, WGSportsLoginRequest source);

        ValueTask<WGSportsPrepareLoginResponse> GetPrepareLoginAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, WGSportsPrepareLoginRequest source);

        ValueTask<WGSportsResponse> LogoutAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, string playerCode);
    }
}
