using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcBg.ApplicationServices;

public class AwcBgPlayerFundCheckingGetService(
    IBrandRepository brand,
    IGameProviderRepository gameProvider,
    IGameInterfaceAwcClient client,
    IPlayerRepository player) : IGetService<AwcBgPlayerFundCheckingQuery, bool>
{
    public async ValueTask<bool> GetAsync(AwcBgPlayerFundCheckingQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await gameProvider.GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.AWC_BG,
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

        var configuration = await gameProvider.GetAwcBgConfigurationAsync(
            gameProviderConfigurationId: gameProviderConfigurationId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await client.FundStatusAsync(
             apiUrl: configuration.ApiUrl,
             agentId: configuration.AgentId,
             cert: configuration.Cert,
             orderId: query.OrderId,
             cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
