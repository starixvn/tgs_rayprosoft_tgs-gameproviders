using System;
using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.Skywind.Models;
using TGS.GameProviders.Domains.SettingConfiguration.TCG;
using TGS.GameProviders.ApiClients.TCG.Models;
using TGS.GameProviders.ApiClients.TCG;

namespace TGS.GameProviders.ApiClients
{
    public interface ITCGApiClient
    {
        ValueTask<TCGKickOutLottoPlayerResponse> KickOutLottoPlayerAsync(Uri ApiUrl, TCGMerchant merchant, TCGKickOutLottoPlayerRequest request);

        ValueTask<TCGLaunchGameResponse> LaunchGameAsync(Uri ApiUrl, TCGMerchant merchant, TCGLaunchGameByELOTTORequest request);

        ValueTask<TCGLaunchGameResponse> LaunchGameAsync(Uri ApiUrl, TCGMerchant merchant, TCGLaunchGameBySEARequest request);

        ValueTask<TCGLaunchGameResponse> LaunchGameAsync(Uri ApiUrl, TCGMerchant merchant, TCGLaunchGameByLOTTOVNRequest request);

        ValueTask<TCGRegisterPlayerResponse> RegisterPlayerAsync(Uri ApiUrl, TCGMerchant merchant, TCGRegisterPlayerRequest request);

        ValueTask<TCGUpdatePasswordPlayerResponse> RegisterPlayerAsync(Uri ApiUrl, TCGMerchant merchant, TCGUpdatePasswordPlayerRequest request);
    }
}
