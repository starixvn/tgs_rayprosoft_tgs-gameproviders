using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.TFGamingService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.TFGamingTransfer.ApplicationServices;

internal class TFGamingTransferGameUrlGetService : IGetService<TFGamingTransferGameUrlQuery, Uri>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceTFGamingClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public TFGamingTransferGameUrlGetService(
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

    public async ValueTask<Uri> GetAsync(TFGamingTransferGameUrlQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.TFGamingTransfer).ConfigureAwait(false);
        var player = await _player.GetAsync(query.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configuration = await _gameProvider.GetTFGamingTransferConfiguration(gameProviderConfigurationId).ConfigureAwait(false);

        var response = await _client
            .GameUrlAsync(
                apiUrl: configuration.ApiUrl,
                privateToken: configuration.PrivateToken,
                memberCode: query.PlayerCode,
                cancellation: cancellationToken)
            .ConfigureAwait(false);

        return new Uri(response.GameUrl);
    }
}
