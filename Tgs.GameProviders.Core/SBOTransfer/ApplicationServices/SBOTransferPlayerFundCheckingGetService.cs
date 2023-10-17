using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.SBOService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SBOTransfer.ApplicationServices;

public class SBOTransferPlayerFundCheckingGetService : IGetService<SBOTransferPlayerFundCheckingQuery, bool>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IGameInterfaceSBOClient _client;
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IPlayerRepository _playerRepository;

    public SBOTransferPlayerFundCheckingGetService(
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

    public async ValueTask<bool> GetAsync(SBOTransferPlayerFundCheckingQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.SBOTransfer,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var player = await _playerRepository.GetAsync(
            playerCode: query.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var gameProviderConfigurationId = await _brandRepository.GetBrandGameProviderConfigurationIdAsync(
            gameProviderId: gameProviderId,
            player.BrandId,
            cancellationToken)
            .ConfigureAwait(false);
        var configuration = await _gameProviderRepository.GetSBOTransferConfigurationAsync(
            gameProviderConfigurationId: gameProviderConfigurationId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await _client
            .FundStatusAsync(
            configuration.ApiUrl,
            configuration.CompanyKey,
            query.OrderId,
            cancellationToken)
            .ConfigureAwait(false);
    }
}
