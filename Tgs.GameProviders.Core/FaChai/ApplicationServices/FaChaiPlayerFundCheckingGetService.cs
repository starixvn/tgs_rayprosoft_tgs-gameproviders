using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.FaChai.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.FaChai.ApplicationServices;

public class FaChaiPlayerFundCheckingGetService(
    IGameProviderRepository gameProvider,
    IPlayerRepository player,
    IBrandRepository brand,
    IGameInterfaceFaChaiClient client) : IGetService<FaChaiPlayerFundCheckingQuery, bool>
{
    public async ValueTask<bool> GetAsync(FaChaiPlayerFundCheckingQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await gameProvider.GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.FaChai,
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
        var configuration = await gameProvider.GetFaChaiConfigurationAsync(
            gameProviderConfigurationId: gameProviderConfigurationId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await client.FundStatusAsync(
            apiUrl: configuration.ApiUrl,
            agentCode: configuration.AgentCode,
            agentKey: configuration.AgentKey,
            currency: configuration.Currency,
            orderId: query.OrderId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
