using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.HabaneroService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.HabaneroTransfer.ApplicationServices;

internal class HabaneroTransferPlayerFundCheckingGetService : IGetService<HabaneroTransferPlayerFundCheckingQuery, bool>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceHabaneroClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

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

    public async ValueTask<bool> GetAsync(HabaneroTransferPlayerFundCheckingQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.HabaneroTransfer,
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

        var configuration = await _gameProvider.GetHabaneroTransferConfigurationAsync(
            gameProviderConfigurationId: gameProviderConfigurationId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return (await _client
            .FundStatusAsync(
            apiUrl: configuration.ApiUrl,
            brandId: configuration.BrandId,
            apiKey: configuration.ApiKey,
            username: player.PlayerCode,
            transactionId: query.OrderId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false))
            .IsOrderExist;
    }
}
