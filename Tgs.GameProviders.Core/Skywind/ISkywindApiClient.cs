using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.Skywind.Models;

namespace Tgs.GameProviders.Core.Skywind
{
    public interface ISkywindApiClient
    {
        ValueTask<SkywindUserLoginResponse> GetAccessTokenAsync(Uri apiUrl, SkywindUserLoginRequest request);

        ValueTask<SkywindGameUrlResponse> GetGameUrlAsync(Uri apiUrl, string getaccessToken, SkywindGameUrlRequest request);

        ValueTask<SkywindGamHistoryRoundDetailsResponse> GetGamHistoryRoundDetailsAsync(Uri apiUrl, string getaccessToken, SkywindGamHistoryRoundDetailsRequest request);
    }
}
