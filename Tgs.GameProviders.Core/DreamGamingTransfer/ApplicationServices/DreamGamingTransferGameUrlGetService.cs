using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.DreamGamingService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.DreamGamingTransfer.ApplicationServices;

public class DreamGamingTransferGameUrlGetService : IGetService<DreamGamingTransferGameUrlQuery, Uri>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceDreamGamingClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public DreamGamingTransferGameUrlGetService(
        IBrandRepository brand,
        IGameProviderRepository gameProvider,
        IGameInterfaceDreamGamingClient client,
        IPlayerRepository player)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public async ValueTask<Uri> GetAsync(DreamGamingTransferGameUrlQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.DreamGamingTransfer).ConfigureAwait(false);
        var player = await _player.GetAsync(query.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configuration = await _gameProvider.GetDreamGamingTransferConfiguration(gameProviderConfigurationId).ConfigureAwait(false);
        var cultureCode = await _gameProvider.GetGameProviderCultureCodeAsync(gameProviderId, query.Language) ?? "en";

        var response = await _client
            .GameUrlAsync(
                apiUrl: configuration.ApiUrl,
                apiKey: configuration.ApiKey,
                agentName: configuration.AgentName,
                username: player.PlayerCode,
                password: "DGTFP@ssw0rd",
                isMobile: query.PlatformType == PlatformType.Mobile,
                lang: cultureCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new Uri(response.GameUrl);
    }
}
