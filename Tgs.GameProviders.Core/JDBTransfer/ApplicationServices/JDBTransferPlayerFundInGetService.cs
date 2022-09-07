using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.JDBService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.JDBTransfer.ApplicationServices;

public class JDBTransferPlayerFundInGetService : IGetService<JDBTransferPlayerFundInQuery, PlayerFundResult>
{
    private readonly IGameInterfaceJDBClient _client;
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IBrandRepository _brandRepository;

    public JDBTransferPlayerFundInGetService(
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

    public async ValueTask<PlayerFundResult> GetAsync(JDBTransferPlayerFundInQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.JDBTransfer).ConfigureAwait(false);
        var player = await _playerRepository.GetAsync(query.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brandRepository.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configuration = await _gameProviderRepository.GetJDBTransferConfiguration(gameProviderConfigurationId).ConfigureAwait(false);

        var reply = await _client
                .FundInAsync(
                    configuration.ApiUrl,
                    configuration.Domain,
                    configuration.AesKey,
                    configuration.AesIV,
                    configuration.Agent,
                    player.PlayerCode,
                    query.OrderId,
                    query.Amount,
                    cancellationToken)
                .ConfigureAwait(false);

        return new PlayerFundResult()
        {
            OrderId = reply.SerialNo
        };
    }
}
