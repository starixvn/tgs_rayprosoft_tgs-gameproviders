using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.BigGaming.Models;
using TGS.GameProviders.ApiClients.BigGaming.Models.Commons;
using TGS.GameProviders.Domains.SettingConfiguration.BigGaming;

namespace TGS.GameProviders.ApiClients.BigGaming
{
    public interface IBigGamingApiClient
    {
        ValueTask<BigGamingResponse> CreateAgentAsync(BigGamingBrandMerchant brandMerchant, BigGamingCreateAgentRequest request);

        ValueTask<BigGamingResponse> CreatePlayerAsync(BigGamingBrandMerchant brandMerchant, BigGamingCreatePlayerRequest request);

        ValueTask<BigGamingResponse> GetGameRecordAsync(BigGamingBrandMerchant brandMerchant, BigGamingGameRecordFishingRequest request);

        ValueTask<BigGamingResponse> GetGameRecordFishingUrlAsync(BigGamingBrandMerchant brandMerchant, BigGamingGameRecordFishingUrlRequest request);

        ValueTask<BigGamingResponse> GetGameUrlAsync(BigGamingBrandMerchant brandMerchant, BigGamingGetGameUrlLiveDealerRequest request);

        ValueTask<BigGamingResponse> GetGameUrlAsync(BigGamingBrandMerchant brandMerchant, BigGamingGetGameUrlFishingRequest request);

        ValueTask<BigGamingResponse> GetTrialGameUrlAsync(BigGamingBrandMerchant brandMerchant, BigGamingGetTrialGameUrlRequest request);

        ValueTask<BigGamingResponse> GetVideoOrderDetailAsync(BigGamingBrandMerchant brandMerchant, BigGamingVideoOrderDetailRequest request);

        ValueTask<BigGamingResponse> GetVideoOrderQueryAsync(BigGamingBrandMerchant brandMerchant, BigGamingVideoOrderQueryRequest request);

        ValueTask<BigGamingResponse> GetVideoRoundQueryAsync(BigGamingBrandMerchant brandMerchant, BigGamingVideoRoundQueryRequest request);

        ValueTask<BigGamingResponse> InfoPlayerAsync(BigGamingBrandMerchant brandMerchant, BigGamingInfoPlayerRequest request);

        ValueTask<BigGamingResponse> KickPlayerAsync(BigGamingBrandMerchant brandMerchant, BigGamingKickPlayerRequest request);
    }
}
