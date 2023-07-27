using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.JDBService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.JDBTransfer.ApplicationServices;

public class JDBTransferPlayerBalanceGetService : IGetService<JDBTransferPlayerBalanceQuery, PlayerBalanceResult>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IGameInterfaceJDBClient _client;
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IPlayerRepository _playerRepository;

    public JDBTransferPlayerBalanceGetService(
        IGameInterfaceJDBClient client,
        IGameProviderRepository gameProviderRepository,
        IPlayerRepository playerRepository,
        IBrandRepository brandRepository)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
    }

    public async ValueTask<PlayerBalanceResult> GetAsync(JDBTransferPlayerBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.JDBTransfer).ConfigureAwait(false);
        var player = await _playerRepository.GetAsync(query.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brandRepository.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configuration = await _gameProviderRepository.GetJDBTransferConfigurationAsync(gameProviderConfigurationId).ConfigureAwait(false);

        var reply = await _client
                .PlayerBalanceAsync(
                    configuration.ApiUrl,
                    configuration.Domain,
                    configuration.AesKey,
                    configuration.AesIV,
                    configuration.Agent,
                    player.PlayerCode,
                    cancellationToken)
                .ConfigureAwait(false);

        if (!reply.IsPlayerExist)
        {
            await _client
               .PlayerCreateAsync(
                   configuration.ApiUrl,
                   configuration.Domain,
                   configuration.AesKey,
                   configuration.AesIV,
                   configuration.Agent,
                   player.PlayerCode,
                   cancellationToken)
               .ConfigureAwait(false);

            reply = await _client
                .PlayerBalanceAsync(
                    configuration.ApiUrl,
                    configuration.Domain,
                    configuration.AesKey,
                    configuration.AesIV,
                    configuration.Agent,
                    player.PlayerCode,
                    cancellationToken)
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
