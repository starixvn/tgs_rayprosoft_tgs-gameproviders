using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcPgs.ApplicationServices;

public class AwcPgsPlayerFundCheckingGetService : IGetService<AwcPgsPlayerFundCheckingQuery, bool>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceAwcClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public AwcPgsPlayerFundCheckingGetService(
        IBrandRepository brand,
        IGameProviderRepository gameProvider,
        IGameInterfaceAwcClient client,
        IPlayerRepository player)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public async ValueTask<bool> GetAsync(AwcPgsPlayerFundCheckingQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.AWC_PGS,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var player = await _player.GetAsync(
            playerCode: query.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(
            gameProviderId: gameProviderId,
            brandId: player.BrandId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var configuration = await _gameProvider.GetAwcPgsConfigurationAsync(
            gameProviderConfigurationId: gameProviderConfigurationId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await _client.FundStatusAsync(
            apiUrl: configuration.ApiUrl,
            agentId: configuration.AgentId,
            cert: configuration.Cert,
            orderId: query.OrderId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
