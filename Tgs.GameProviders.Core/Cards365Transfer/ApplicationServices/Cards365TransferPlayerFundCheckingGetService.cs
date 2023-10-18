using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Cards365.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Cards365Transfer.ApplicationServices;

public class Cards365TransferPlayerFundCheckingGetService : IGetService<Cards365TransferPlayerFundCheckingQuery, bool>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceCards365Client _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public Cards365TransferPlayerFundCheckingGetService(
        IGameProviderRepository gameProvider,
        IPlayerRepository player,
        IBrandRepository brand,
        IGameInterfaceCards365Client client)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public async ValueTask<bool> GetAsync(Cards365TransferPlayerFundCheckingQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.Cards365Transfer,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var player = await _player.GetAsync(
            playerCode: query.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(
            gameProviderId: gameProviderId,
            brandId: player.BrandId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var configuration = await _gameProvider.GetCards365TransferConfigurationAsync(
            gameProviderConfigurationId: gameProviderConfigurationId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await _client
            .FundStatusAsync(
            apiUrl: configuration.ApiUrl,
            website: configuration.Website,
            cert: configuration.Cert,
            transactionId: query.OrderId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
