using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.Card365;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.Card365;

internal class Card365Service : ICard365Service
{
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly ILogger<Card365Service> _logger;
    private readonly ISettingService _settingService;

    public Card365Service(
        ILogger<Card365Service> logger,
        IGameProviderRepository gameProviderRepository,
        ISettingService settingService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
    }

    public async ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode)
    {
        var (isCurrencyCode, gameProviderCurrencyCode) = await _gameProviderRepository
            .GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode)
            .ConfigureAwait(false);

        return isCurrencyCode ? gameProviderCurrencyCode : null;
    }

    public async ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode)
    {
        var (isCultureCode, gameProviderCultureCode) = await _gameProviderRepository
            .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode)
            .ConfigureAwait(false);

        return isCultureCode ? gameProviderCultureCode : "EN ";
    }

    public async ValueTask<Card365Configuration> GetSettingConfigurationAsync()
        => await _settingService
            .GetConfigurationAsync<Card365Configuration>(GameProviderConfigurations.Card365)
            .ConfigureAwait(false);
}
