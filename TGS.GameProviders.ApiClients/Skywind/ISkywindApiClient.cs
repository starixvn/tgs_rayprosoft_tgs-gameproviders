using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.Skywind.Models;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.ApiClients.Skywind
{
    public interface ISkywindApiClient
    {
        ValueTask<SkywindGameUrlResponse> GetGameUrlAsync(SkywindConfiguration configuration, string getaccessToken, SkywindGameUrlRequest request);

        ValueTask<SkywindGamHistoryRoundDetailsResponse> GetGamHistoryRoundDetailsAsync(SkywindConfiguration configuration, string getaccessToken, SkywindGamHistoryRoundDetailsRequest request);

        ValueTask<SkywindUserLoginResponse> GetAccessTokenAsync(SkywindConfiguration configuration);
    }
}
