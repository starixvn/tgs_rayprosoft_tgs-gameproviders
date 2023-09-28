using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.OBet.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.OBet.ApplicationServices;

public class OBetPlayerFundCheckingGetService : IGetService<OBetPlayerFundCheckingQuery, bool>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceOBetClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public OBetPlayerFundCheckingGetService(
        IGameProviderRepository gameProvider,
        IPlayerRepository player,
        IBrandRepository brand,
        IGameInterfaceOBetClient client)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public async ValueTask<bool> GetAsync(OBetPlayerFundCheckingQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider
            .GetGameProviderIdAsync(
                gameProviderCode: GameProviderType.OBet,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var player = await _player
            .GetAsync(
                playerCode: query.PlayerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand
            .GetBrandGameProviderConfigurationIdAsync(
                gameProviderId: gameProviderId,
                brandId: player.BrandId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var configuration = await _gameProvider
            .GetOBetConfigurationAsync(
                gameProviderConfigurationId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await _client
            .FundStatusAsync(
            apiUrl: configuration.ApiUrl,
            apiKey: configuration.ApiKey,
            agentId: configuration.AgentId,
            playerCode: player.PlayerCode,
            serialNo: query.OrderId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
