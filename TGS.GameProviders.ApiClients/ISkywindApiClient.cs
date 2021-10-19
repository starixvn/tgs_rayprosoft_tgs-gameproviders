using System;
using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.Skywind.Models;

namespace TGS.GameProviders.ApiClients
{
    public interface ISkywindApiClient
    {
        ValueTask<SkywindUserLoginResponse> GetAccessTokenAsync(Uri apiUrl, SkywindUserLoginRequest request);

        ValueTask<SkywindGameUrlResponse> GetGameUrlAsync(Uri apiUrl, string getaccessToken, SkywindGameUrlRequest request);

        ValueTask<SkywindGamHistoryRoundDetailsResponse> GetGamHistoryRoundDetailsAsync(Uri apiUrl, string getaccessToken, SkywindGamHistoryRoundDetailsRequest request);
    }
}
