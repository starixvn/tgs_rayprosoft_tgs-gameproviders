using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.IMESports;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Services.IMESports;

internal class IMESportsService : IIMESportsService
{
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IGameRepository _gameRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly ISettingRepository _settingRepository;

    public IMESportsService(
        ISettingRepository settingRepository,
        IGameProviderRepository gameProviderRepository,
        IGameRepository gameRepository,
        IPlayerRepository playerRepository)
    {
        _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
        _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
    }

    public async ValueTask<GameModel> GetGameAsync(int gameId)
        => await _gameRepository.GetAsync(gameId).ConfigureAwait(false);

    public async ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode)
    {
        var gameProviderCurrencyCode = await _gameProviderRepository
            .GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode)
            .ConfigureAwait(false);

        return !string.IsNullOrEmpty(gameProviderCurrencyCode) ? gameProviderCurrencyCode : null;
    }

    public ValueTask<Guid> GetGameProviderIdAsync()
        => _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.IMESports);

    public async ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode)
    {
        var gameProviderCultureCode = await _gameProviderRepository
            .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode)
            .ConfigureAwait(false);

        return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "EN";
    }

    public async ValueTask<PlayerModel> GetPlayerAsync(string playerCode)
        => await _playerRepository.GetAsync(playerCode).ConfigureAwait(false);

    public async ValueTask<IMESportsConfiguration> GetSettingConfigurationAsync()
        => await _settingRepository
            .GetConfigurationAsync<IMESportsConfiguration>(GameProviderConfigurations.IMESports)
            .ConfigureAwait(false);
}
