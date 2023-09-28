using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Jili.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.JiliTransfer.ApplicationServices;

public class JiliTransferPlayerFundCheckingGetService : IGetService<JiliTransferPlayerFundCheckingQuery, bool>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceJiliClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public JiliTransferPlayerFundCheckingGetService(
        IBrandRepository brand,
        IGameProviderRepository gameProvider,
        IGameInterfaceJiliClient client,
        IPlayerRepository player)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public async ValueTask<bool> GetAsync(JiliTransferPlayerFundCheckingQuery command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider
            .GetGameProviderIdAsync(
                gameProviderCode: GameProviderType.JiliTransfer,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var player = await _player
            .GetAsync(
                playerCode: command.PlayerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand
            .GetBrandGameProviderConfigurationIdAsync(
                gameProviderId: gameProviderId,
                brandId: player.BrandId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var configuration = await _gameProvider
            .GetJiliTransferConfigurationAsync(
                gameProviderConfigurationId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await _client
           .FundOrderCheckingAsync(
           apiUrl: configuration.ApiUrl,
           agentId: configuration.AgentId,
           agentKey: configuration.AgentKey,
           transactionId: command.OrderId,
           cancellationToken: cancellationToken)
           .ConfigureAwait(false);
    }
}
