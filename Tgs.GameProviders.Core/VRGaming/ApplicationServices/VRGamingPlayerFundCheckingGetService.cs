using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.VRGaming.Client;
using Tgs.GameProviders.Domains;

using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.VRGaming.ApplicationServices;

public class VRGamingPlayerFundCheckingGetService(
    IGameProviderRepository gameProvider,
    IPlayerRepository player,
    IBrandRepository brand,
    IGameInterfaceVRGamingClient client) : IGetService<VRGamingPlayerFundCheckingQuery, bool>
{
    public async ValueTask<bool> GetAsync(VRGamingPlayerFundCheckingQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await gameProvider.GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.VRGaming,
            cancellationToken: cancellationToken)
           .ConfigureAwait(false);
        var playerInfo = await player.GetAsync(
            playerCode: query.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var gameProviderConfigurationId = await brand.GetBrandGameProviderConfigurationIdAsync(
            gameProviderId: gameProviderId,
            brandId: playerInfo.BrandId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var configuration = await gameProvider.GetVRGamingConfigurationAsync(
            gameProviderConfigurationId: gameProviderConfigurationId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await client.FundStatusAsync(
             configuration.ApiUrl,
             configuration.Version,
             configuration.MerchantId,
             configuration.Deskey,
             playerInfo.PlayerCode,
             orderId: query.OrderId,
             startTime: query.StartTime,
             endTime: query.EndTime,
             cancellationToken: cancellationToken)
             .ConfigureAwait(false);
    }
}
