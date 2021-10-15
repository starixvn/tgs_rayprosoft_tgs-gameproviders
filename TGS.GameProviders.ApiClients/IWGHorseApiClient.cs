using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.WGHorse.Models;
using TGS.GameProviders.ApiClients.WGHorse.Models.Commons;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.ApiClients
{
    public interface IWGHorseApiClient
    {
        ValueTask<WGHorseResponse> AccessMoneyAsync(WGHorseConfiguration configuration, WGHorseAccessMoneyRequest source);

        ValueTask<WGHorseResponse> AccessMoneyQueryAsync(WGHorseConfiguration configuration, WGHorseAccessMoneyQueryRequest source);

        ValueTask<WGHorseGetBalanceResponse> GetBalanceAsync(WGHorseConfiguration configuration, WGHorseGetBalanceRequest source);

        ValueTask<WGHorseGameRecordResponse> GetGameRecordAsync(WGHorseConfiguration configuration, WGHorseGameRecordRequest source);

        ValueTask<WGHorseLoginResponse> GetLoginAsync(WGHorseConfiguration configuration, WGHorseLoginRequest source);

        ValueTask<WGHorsePrepareLoginResponse> GetPrepareLoginAsync(WGHorseConfiguration configuration, WGHorsePrepareLoginRequest source);
    }
}
