using System;
using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.WMCasino.Models;

namespace TGS.GameProviders.ApiClients
{
    public interface IWMCasinoApiClient
    {
        ValueTask<WMCasinoLoginGameResponse> LoginGameAsync(Uri apiUrl, WMCasinoLoginGameRequest request);

        ValueTask<WMCasinoLogoutGameResponse> LogoutGameAsync(Uri apiUrl, WMCasinoLogoutGameRequest request);

        ValueTask<WMCasinoMemberRegisterResponse>
            MemberRegisterAsync(Uri apiUrl, WMCasinoMemberRegisterRequest request);

        ValueTask<WMCasinoGetDateTimeReportResponse> GetDateTimeReportAsync(Uri apiUrl,
            WMCasinoGetDateTimeReportRequest request);
    }
}