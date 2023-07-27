using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.TFGamingService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.TFGamingTransfer.ApplicationServices;

public class TFGamingTransferPlayerBalanceGetService : IGetService<TFGamingTransferPlayerBalanceQuery, PlayerBalanceResult>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceTFGamingClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public TFGamingTransferPlayerBalanceGetService(
        IBrandRepository brand,
        IGameProviderRepository gameProvider,
        IGameInterfaceTFGamingClient client,
        IPlayerRepository player)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public async ValueTask<PlayerBalanceResult> GetAsync(TFGamingTransferPlayerBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider
            .GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.TFGamingTransfer,
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
            .GetTFGamingTransferConfigurationAsync(
            gameProviderConfigurationId: gameProviderConfigurationId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var reply = await _client
            .PlayerBalanceAsync(
                apiUrl: configuration.ApiUrl,
                privateToken: configuration.PrivateToken,
                memberCode: player.PlayerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!reply.IsPlayerExist)
        {
            await _client
                .PlayerCreateAsync(
                    apiUrl: configuration.ApiUrl,
                    privateToken: configuration.PrivateToken,
                    memberCode: player.PlayerCode,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            reply = await _client
               .PlayerBalanceAsync(
                   apiUrl: configuration.ApiUrl,
                   privateToken: configuration.PrivateToken,
                   memberCode: player.PlayerCode,
                   cancellationToken: cancellationToken)
               .ConfigureAwait(false);
        }

        return !reply.IsPlayerExist
            ? throw new Exception($"Player does not exist.{player.PlayerCode}")
            : new PlayerBalanceResult
            {
                FreeBalance = reply.Balance.ToDecimal().GetValueOrDefault(0),
                OccupyBalance = 0
            };
    }
}
