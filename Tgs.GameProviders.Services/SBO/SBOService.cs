using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.SBO;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Services.SBO;

internal class SBOService : ISBOService
{
    private readonly IGameProviderRepository _gameProviderService;
    private readonly IGameRepository _gameRepository;
    private readonly ISettingRepository _settingService;

    public SBOService(
        ISettingRepository settingService,
        IGameProviderRepository gameProviderService,
        IGameRepository gameRepository)
    {
        _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
        _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
        _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
    }

    public async ValueTask<GameModel> GetGameAsync(int gameId)
        => await _gameRepository.GetAsync(gameId).ConfigureAwait(false);

    public async ValueTask<string> GetGameProviderCultureCodeAsync(Guid gameProviderId, string cultureCode)
    {
        var gameProviderCultureCode = await _gameProviderService
            .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);

        return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "en_US";
    }

    public async ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode)
    {
        var gameProviderCurrencyCode = await _gameProviderService
            .GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode)
            .ConfigureAwait(false);

        return !string.IsNullOrEmpty(gameProviderCurrencyCode) ? gameProviderCurrencyCode : null;
    }

    public ValueTask<Guid> GetGameProviderIdAsync()
        => _gameProviderService.GetGameProviderIdAsync(GameProviderType.SBOBET);

    public async ValueTask<SBOConfiguration> GetSettingConfigurationAsync()
        => await _settingService.GetConfigurationAsync<SBOConfiguration>(GameProviderConfigurations.SBOBET)
            .ConfigureAwait(false);
}
