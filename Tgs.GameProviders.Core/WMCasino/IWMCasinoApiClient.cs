using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.WMCasino.Models;

namespace Tgs.GameProviders.Core.WMCasino
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
