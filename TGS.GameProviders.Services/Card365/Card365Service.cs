using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.Card365;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.Card365;

internal class Card365Service : ICard365Service
{
    private readonly IGameProviderService _gameProviderService;
    private readonly ILogger<Card365Service> _logger;
    private readonly ISettingService _settingService;

    public Card365Service(
        ILogger<Card365Service> logger,
        IGameProviderService gameProviderService,
        ISettingService settingService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
        _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
    }

    public async ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode)
    {
        var gameProviderCurrencyCode = await _gameProviderService
            .GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode)
            .ConfigureAwait(false);

        return !string.IsNullOrEmpty(gameProviderCurrencyCode) ? gameProviderCurrencyCode : null;
    }

    public async ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode)
    {
        var gameProviderCultureCode = await _gameProviderService
            .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode)
            .ConfigureAwait(false);

        return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "EN ";
    }

    public async ValueTask<Card365Configuration> GetSettingConfigurationAsync()
        => await _settingService
            .GetConfigurationAsync<Card365Configuration>(GameProviderConfigurations.Card365)
            .ConfigureAwait(false);
}
