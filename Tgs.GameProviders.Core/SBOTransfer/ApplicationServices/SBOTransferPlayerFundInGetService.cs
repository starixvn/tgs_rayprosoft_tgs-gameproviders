using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.SBOService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SBOTransfer.ApplicationServices;

public class SBOTransferPlayerFundInGetService : IGetService<SBOTransferPlayerFundInQuery, PlayerFundResult>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IGameInterfaceSBOClient _client;
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IPlayerRepository _playerRepository;

    public SBOTransferPlayerFundInGetService(
        IGameInterfaceSBOClient client,
        IGameProviderRepository gameProviderRepository,
        IPlayerRepository playerRepository,
        IBrandRepository brandRepository)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
    }

    public async ValueTask<PlayerFundResult> GetAsync(SBOTransferPlayerFundInQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.SBOTransfer).ConfigureAwait(false);
        var player = await _playerRepository.GetAsync(query.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brandRepository.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configuration = await _gameProviderRepository.GetSBOTransferConfigurationAsync(gameProviderConfigurationId).ConfigureAwait(false);

        var reply = await _client
                .FundInAsync(
                    configuration.ApiUrl,
                    configuration.CompanyKey,
                    configuration.Agent,
                    player.PlayerCode,
                    query.Amount,
                    cancellationToken)
                .ConfigureAwait(false);

        return new PlayerFundResult()
        {
            OrderId = reply.TxnId
        };
    }
}
