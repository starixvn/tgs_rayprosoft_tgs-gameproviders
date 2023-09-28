using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcSpade.ApplicationServices;

public class AwcSpadePlayerFundCheckingGetService : IGetService<AwcSpadePlayerFundCheckingQuery, bool>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceAwcServiceClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public AwcSpadePlayerFundCheckingGetService(
        IBrandRepository brand,
        IGameProviderRepository gameProvider,
        IGameInterfaceAwcServiceClient client,
        IPlayerRepository player)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public async ValueTask<bool> GetAsync(AwcSpadePlayerFundCheckingQuery command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider
            .GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.AWC_SPADE,
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
            .GetAwcSpadeConfigurationAsync(
            gameProviderConfigurationId: gameProviderConfigurationId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await _client
            .FundOrderCheckingAsync(
            apiUrl: configuration.ApiUrl,
            agentId: configuration.AgentId,
            cert: configuration.Cert,
            transactionId: command.OrderId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
