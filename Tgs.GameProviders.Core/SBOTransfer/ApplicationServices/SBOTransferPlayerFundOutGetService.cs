using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.SBOService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SBOTransfer.ApplicationServices;

public class SBOTransferPlayerFundOutGetService : IGetService<SBOTransferPlayerFundOutQuery, PlayerFundResult>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IGameInterfaceSBOClient _client;
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly ISnowflakeWork _snowflakeWork;

    public SBOTransferPlayerFundOutGetService(
        IGameInterfaceSBOClient client,
        IGameProviderRepository gameProviderRepository,
        IPlayerRepository playerRepository,
        IBrandRepository brandRepository,
        ISnowflakeWork snowflakeWork)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
        _snowflakeWork = snowflakeWork ?? throw new ArgumentNullException(nameof(snowflakeWork));
    }

    public async ValueTask<PlayerFundResult> GetAsync(SBOTransferPlayerFundOutQuery query, CancellationToken cancellationToken = default)
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

        var orderId = $"D{_snowflakeWork.GetId()}";

        var reply = await _client.FundOutAsync(
            apiUrl: configuration.ApiUrl,
            companyKey: configuration.CompanyKey,
            agent: configuration.Agent,
            playerCode: player.PlayerCode,
            orderId: orderId,
            amount: query.Amount,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new PlayerFundResult()
        {
            OrderId = reply.TxnId
        };
    }
}
