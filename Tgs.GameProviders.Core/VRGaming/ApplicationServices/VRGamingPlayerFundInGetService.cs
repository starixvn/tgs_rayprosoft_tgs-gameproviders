using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using Tgs.GameInterface.VRGaming.Client;
using Tgs.GameProviders.Domains;

namespace Tgs.GameProviders.Core.VRGaming.ApplicationServices;

public class VRGamingPlayerFundInGetService(
    IGameProviderRepository gameProvider,
    IPlayerRepository player,
    IBrandRepository brand,
    IGameInterfaceVRGamingClient client) : IGetService<VRGamingPlayerFundInQuery, PlayerFundResult>
{
    public async ValueTask<PlayerFundResult> GetAsync(VRGamingPlayerFundInQuery query, CancellationToken cancellationToken = default)
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

        var reply = await client.FundInAsync(
             configuration.ApiUrl,
             configuration.Version,
             configuration.MerchantId,
             configuration.Deskey,
             playerInfo.PlayerCode,
             orderId: query.OrderId,
             amount: query.Amount,
             cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new PlayerFundResult
        {
            OrderId = reply.OrderId
        };
    }
}
