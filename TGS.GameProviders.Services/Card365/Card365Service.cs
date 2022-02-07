using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.Card365;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.Card365;

internal class Card365Service : ICard365Service
{
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly ILogger<Card365Service> _logger;
    private readonly ISettingRepository _settingRepository;

    public Card365Service(
        ILogger<Card365Service> logger,
        IGameProviderRepository gameProviderRepository,
        ISettingRepository settingRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gameProviderRepository = gameProviderRepository;
        _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
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
        => await _settingRepository
            .GetConfigurationAsync<Card365Configuration>(GameProviderConfigurations.Card365)
            .ConfigureAwait(false);
}
