using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.YeeBet.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.YeeBet.ApplicationServices;

public class YeeBetPlayerBalanceGetService : IGetService<YeeBetPlayerBalanceQuery, decimal?>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceYeeBetClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;
    private readonly ICommandService<YeeBetPlayerCreateCommand> _playerCreateService;

    public YeeBetPlayerBalanceGetService(
        IGameProviderRepository gameProvider,
        IPlayerRepository player,
        IBrandRepository brand,
        IGameInterfaceYeeBetClient client,
        ICommandService<YeeBetPlayerCreateCommand> playerCreateService)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _playerCreateService = playerCreateService ?? throw new ArgumentNullException(nameof(playerCreateService));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public async ValueTask<decimal?> GetAsync(YeeBetPlayerBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider
            .GetGameProviderIdAsync(
                gameProviderCode: GameProviderType.YeeBet,
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
            .GetYeeBetConfigurationAsync(
                gameProviderConfigurationId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var reply = await _client
            .PlayerBalanceAsync(
                apiUrl: configuration.ApiUrl,
                appId: configuration.AppId,
                secretKey: configuration.SecretKey,
                playerCode: player.PlayerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!reply.IsPlayerExist)
        {
            await _playerCreateService
                .ExecuteAsync(new YeeBetPlayerCreateCommand(player.PlayerCode), cancellationToken)
                .ConfigureAwait(false);

            reply = await _client
               .PlayerBalanceAsync(
                    apiUrl: configuration.ApiUrl,
                    appId: configuration.AppId,
                    secretKey: configuration.SecretKey,
                    playerCode: player.PlayerCode,
                    cancellationToken: cancellationToken)
               .ConfigureAwait(false);
        }

        return reply.IsPlayerExist ? reply.Balance : null;
    }
}
