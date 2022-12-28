using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.TFGamingService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.TFGamingTransfer.ApplicationServices;

public class TFGamingTransferPlayerFundOutGetService : IGetService<TFGamingTransferPlayerFundOutQuery, PlayerFundResult>
{
    private readonly IBrandRepository _brand;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IGameInterfaceTFGamingClient _client;
    private readonly IPlayerRepository _player;

    public TFGamingTransferPlayerFundOutGetService(
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

    public async ValueTask<PlayerFundResult> GetAsync(TFGamingTransferPlayerFundOutQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.TFGamingTransfer).ConfigureAwait(false);
        var player = await _player.GetAsync(query.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configuration = await _gameProvider.GetTFGamgingTransferConfiguration(gameProviderConfigurationId).ConfigureAwait(false);

        var reply = await _client
            .FundOutAsync(
                apiUrl: configuration.ApiUrl,
                operatorId: configuration.OperatorId,
                privateToken: configuration.PrivateToken,
                member: player.PlayerCode,
                refTransactionCode: query.OrderId,
                amount: query.Amount,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new PlayerFundResult()
        {
            OrderId = reply.TransactionId
        };
    }
}
