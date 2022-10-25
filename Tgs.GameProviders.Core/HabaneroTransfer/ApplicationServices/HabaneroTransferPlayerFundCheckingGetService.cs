using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.HabaneroService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.HabaneroTransfer.ApplicationServices;

internal class HabaneroTransferPlayerFundCheckingGetService : IGetService<HabaneroTransferPlayerFundCheckingQuery, PlayerFundCheckingResult>
{
    private readonly IBrandRepository _brand;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;
    private readonly IGameInterfaceHabaneroClient _client;

    public HabaneroTransferPlayerFundCheckingGetService(
        IBrandRepository brand,
        IGameProviderRepository gameProvider,
        IPlayerRepository player,
        IGameInterfaceHabaneroClient client)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<PlayerFundCheckingResult> GetAsync(HabaneroTransferPlayerFundCheckingQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.HabaneroTransfer).ConfigureAwait(false);
        var player = await _player.GetAsync(query.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configuration = await _gameProvider.GetHabaneroTransferConfiguration(gameProviderConfigurationId).ConfigureAwait(false);

        var response = await _client
            .FundStatusAsync(
                apiUrl: configuration.ApiUrl,
                brandId: configuration.BrandId,
                apiKey: configuration.ApiKey,
                username: player.PlayerCode,
                transactionId: query.OrderId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new PlayerFundCheckingResult { OrderStatus = response.IsOrderExist };
    }
}
