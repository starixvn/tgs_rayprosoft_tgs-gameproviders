using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.DreamGame;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Services.DreamGame;

internal class DreamGameService : IDreamGameService
{
    private readonly IGameProviderRepository _gameProviderService;
    private readonly ISettingRepository _settingService;

    public DreamGameService(
    IGameProviderRepository gameProviderService,
    ISettingRepository settingService)
    {
        _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
        _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
    }

    public async ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode)
    {
        var gameProviderId = await GetGameProviderIdAsync().ConfigureAwait(false);
        var gameProviderCurrencyCode = await _gameProviderService
            .GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode)
            .ConfigureAwait(false);

        return !string.IsNullOrEmpty(gameProviderCurrencyCode) ? gameProviderCurrencyCode : null;
    }

    public ValueTask<Guid> GetGameProviderIdAsync()
        => _gameProviderService.GetGameProviderIdAsync(GameProviderType.DreamGame);

    public async ValueTask<string> GetGameProviderLanguageCodeAsync(string cultureCode)
    {
        var gameProviderId = await GetGameProviderIdAsync().ConfigureAwait(false);
        var gameProviderCultureCode = await _gameProviderService
            .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode)
            .ConfigureAwait(false);

        return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "en ";
    }

    public async ValueTask<DreamGameConfiguration> GetSettingConfigurationAsync()
        => await _settingService
            .GetConfigurationAsync<DreamGameConfiguration>(GameProviderConfigurations.DreamGame)
            .ConfigureAwait(false);
}
