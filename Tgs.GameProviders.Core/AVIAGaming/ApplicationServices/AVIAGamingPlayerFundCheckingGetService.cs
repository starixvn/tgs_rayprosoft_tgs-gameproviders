using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.AVIAGaming.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AVIAGaming.ApplicationServices;

public class AVIAGamingPlayerFundCheckingGetService : IGetService<AVIAGamingPlayerFundCheckingQuery, bool>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceAVIAGamingClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public AVIAGamingPlayerFundCheckingGetService(
        IGameProviderRepository gameProvider,
        IPlayerRepository player,
        IBrandRepository brand,
        IGameInterfaceAVIAGamingClient client)
    {
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<bool> GetAsync(AVIAGamingPlayerFundCheckingQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider
            .GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.AVIAGaming,
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
            .GetAVIAGamingConfigurationAsync(
            gameProviderConfigurationId: gameProviderConfigurationId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await _client
             .FundStatusAsync(
             apiUrl: configuration.ApiUrl,
             authorization: configuration.Authorization,
             orderId: query.OrderId,
             cancellationToken: cancellationToken)
             .ConfigureAwait(false);
    }
}
