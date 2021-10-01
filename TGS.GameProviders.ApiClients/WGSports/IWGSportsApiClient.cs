using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.WGSports.Models;
using TGS.GameProviders.ApiClients.WGSports.Models.Commons;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.ApiClients.WGSports
{
    public interface IWGSportsApiClient
    {
        ValueTask<WGSportsResponse> AccessMoneyAsync(WGSportsConfiguration configuration, WGSportsAccessMoneyRequest source);

        ValueTask<WGSportsResponse> AccessMoneyQueryAsync(WGSportsConfiguration configuration, WGSportsAccessMoneyQueryRequest source);

        ValueTask<WGSportsGetBalanceResponse> GetBalanceAsync(WGSportsConfiguration configuration, WGSportsGetBalanceRequest source);

        ValueTask<WGSportsGameRecordResponse> GetGameRecordAsync(WGSportsConfiguration configuration, WGSportsGameRecordRequest source);

        ValueTask<WGSportsLoginResponse> GetLoginAsync(WGSportsConfiguration configuration, WGSportsLoginRequest source);

        ValueTask<WGSportsPrepareLoginResponse> GetPrepareLoginAsync(WGSportsConfiguration configuration, WGSportsPrepareLoginRequest source);
    }
}
