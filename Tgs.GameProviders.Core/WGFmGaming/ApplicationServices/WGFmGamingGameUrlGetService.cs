using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WGFmGaming.ApplicationServices;

public class WGFmGamingGameUrlGetService : IGetService<WGFmGamingGameUrlQuery, Uri>
{
    private readonly IGameInterfaceWGFmGamingClient _client;
    private readonly ISettingRepository _settingRepository;
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IPlayerRepository _playerRepository;

    public WGFmGamingGameUrlGetService(
        IGameInterfaceWGFmGamingClient client,
        ISettingRepository settingRepository,
        IGameProviderRepository gameProviderRepository,
        IPlayerRepository playerRepository)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
        _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
    }

    public async ValueTask<Uri> GetAsync(WGFmGamingGameUrlQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.WGSports).ConfigureAwait(false);
        var configuration = await _settingRepository.GetWGSportsConfigurationAsync().ConfigureAwait(false);
        var playerModel = await _playerRepository.GetAsync(query.PlayerCode).ConfigureAwait(false);
        var cultureCode = await _gameProviderRepository
            .GetGameProviderCultureCodeAsync(
                gameProviderId,
                query.Language)
            .ConfigureAwait(false) ?? "2";

        var currencyCode = await _gameProviderRepository
            .GetGameProviderCurrencyCodeAsync(
                gameProviderId,
                playerModel.DefaultWalletCurrencyCode)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game product currency does not exist.({query.CurrencyCode})");

        var token = await _client
            .GamePrepareLoginAsync(
                configuration.ApiUrl,
                configuration.PartnerCode,
                configuration.PartnerKey,
                configuration.IV,
                playerModel.PlayerCode,
                currencyCode,
                cancellationToken)
            .ConfigureAwait(false);

        return await _client
            .GameLoginAsync(
                configuration.ApiUrl,
                configuration.PartnerCode,
                token,
                cultureCode,
                cancellationToken)
            .ConfigureAwait(false);
    }
}
