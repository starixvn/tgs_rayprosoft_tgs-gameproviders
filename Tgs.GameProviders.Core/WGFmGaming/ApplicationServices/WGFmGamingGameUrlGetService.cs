using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WGFmGaming.ApplicationServices;

public class WGFmGamingGameUrlGetService : IGetService<WGFmGamingGameUrlQuery, Uri>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IGameInterfaceWGFmGamingClient _client;
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IPlayerRepository _playerRepository;

    public WGFmGamingGameUrlGetService(
        IGameInterfaceWGFmGamingClient client,
        IGameProviderRepository gameProviderRepository,
        IPlayerRepository playerRepository,
        IBrandRepository brandRepository)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
    }

    public async ValueTask<Uri> GetAsync(WGFmGamingGameUrlQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.WGSports).ConfigureAwait(false);
        var player = await _playerRepository.GetAsync(query.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brandRepository.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configurationData = await _gameProviderRepository.GetWGFmGamingConfiguration(gameProviderConfigurationId).ConfigureAwait(false);
        var cultureCode = await _gameProviderRepository
            .GetGameProviderCultureCodeAsync(
                gameProviderId,
                query.Language)
            .ConfigureAwait(false) ?? "2";

        var currencyCode = await _gameProviderRepository
            .GetGameProviderCurrencyCodeAsync(
                gameProviderId,
                player.DefaultWalletCurrencyCode)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game product currency does not exist.({query.CurrencyCode})");

        var token = await _client
            .GamePrepareLoginAsync(
                configurationData.ApiUrl,
                configurationData.PartnerCode,
                configurationData.PartnerKey,
                configurationData.IV,
                player.PlayerCode,
                currencyCode,
                cancellationToken)
            .ConfigureAwait(false);

        return await _client
            .GameLoginAsync(
                configurationData.ApiUrl,
                configurationData.PartnerCode,
                token,
                cultureCode,
                configurationData.Version,
                cancellationToken)
            .ConfigureAwait(false);
    }
}
