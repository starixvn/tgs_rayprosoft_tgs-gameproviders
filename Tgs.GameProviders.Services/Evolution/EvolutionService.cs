using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.Evolution;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Services.Evolution;

internal class EvolutionService : IEvolutionService
{
    private readonly IGameProviderRepository _gameProviderService;
    private readonly ISettingRepository _settingService;

    public EvolutionService(
        ISettingRepository settingService,
        IGameProviderRepository gameProviderService
        )
    {
        _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
        _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
    }

    public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
    {
        var gameProviderId = await _gameProviderService.GetGameProviderIdAsync(GameProviderType.Evolution).ConfigureAwait(false);
        var gameProviderCultureCode = await _gameProviderService.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);
        return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "en";
    }

    public async ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode)
    {
        var gameProviderId = await _gameProviderService.GetGameProviderIdAsync(GameProviderType.Evolution).ConfigureAwait(false);
        var gameProviderCurrencyCode = await _gameProviderService.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode).ConfigureAwait(false);
        return !string.IsNullOrEmpty(gameProviderCurrencyCode) ? gameProviderCurrencyCode : null;
    }

    public ValueTask<EvolutionConfiguration> GetSettingConfigurationAsync()
        => _settingService.GetConfigurationAsync<EvolutionConfiguration>(GameProviderConfigurations.Evolution);
}
