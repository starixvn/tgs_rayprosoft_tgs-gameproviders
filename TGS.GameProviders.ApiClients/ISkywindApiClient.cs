using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.Skywind.Models;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.ApiClients.Skywind;

namespace TGS.GameProviders.ApiClients
{
    public interface ISkywindApiClient
    {
        ValueTask<SkywindUserLoginResponse> GetAccessTokenAsync(SkywindConfiguration configuration);

        ValueTask<SkywindGameUrlResponse> GetGameUrlAsync(SkywindConfiguration configuration, string getaccessToken, SkywindGameUrlRequest request);

        ValueTask<SkywindGamHistoryRoundDetailsResponse> GetGamHistoryRoundDetailsAsync(SkywindConfiguration configuration, string getaccessToken, SkywindGamHistoryRoundDetailsRequest request);
    }
}
