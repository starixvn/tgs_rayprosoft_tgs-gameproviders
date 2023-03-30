using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.OBet.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.OBet.ApplicationServices;

public class OBetPlayerBalanceGetService : IGetService<OBetPlayerBalanceQuery, decimal?>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceOBetClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public OBetPlayerBalanceGetService(
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

    public async ValueTask<decimal?> GetAsync(OBetPlayerBalanceQuery query, CancellationToken cancellationToken = default)
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

        var reply = await _client
            .PlayerBalanceAsync(
                apiUrl: configuration.ApiUrl,
                apiKey: configuration.ApiKey,
                agentId: configuration.AgentId,
                playerCode: player.PlayerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!reply.IsPlayerExist)
        {
            await _client
                .PlayerCreateAsync(
                    apiUrl: configuration.ApiUrl,
                    apiKey: configuration.ApiKey,
                    agentId: configuration.AgentId,
                    playerCode: player.PlayerCode,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            reply = await _client
               .PlayerBalanceAsync(
                    apiUrl: configuration.ApiUrl,
                    apiKey: configuration.ApiKey,
                    agentId: configuration.AgentId,
                    playerCode: player.PlayerCode,
                    cancellationToken: cancellationToken)
               .ConfigureAwait(false);
        }

        return reply.IsPlayerExist ? reply.Balance : null;
    }
}
