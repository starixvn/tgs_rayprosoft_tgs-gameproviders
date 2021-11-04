using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.WMCasino.Models;

namespace TGS.GameProviders.Core.WMCasino
{
    public interface IWMCasinoApiClient
    {
        ValueTask<WMCasinoGetDateTimeReportResponse> GetDateTimeReportAsync(Uri apiUrl,
            WMCasinoGetDateTimeReportRequest request);

        ValueTask<WMCasinoLoginGameResponse> LoginGameAsync(Uri apiUrl, WMCasinoLoginGameRequest request);

        ValueTask<WMCasinoLogoutGameResponse> LogoutGameAsync(Uri apiUrl, WMCasinoLogoutGameRequest request);

        ValueTask<WMCasinoMemberRegisterResponse>
            MemberRegisterAsync(Uri apiUrl, WMCasinoMemberRegisterRequest request);
    }
}
