using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.SexyGaming;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.SexyGaming
{
    internal class SexyGamingService : ISexyGamingService
    {
        private readonly ISettingService _settingService;
        private readonly IGameProviderService _gameProviderService;

        public SexyGamingService(
            ISettingService settingService,
            IGameProviderService gameProviderService
            )
        {
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
        }


        public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
        {
            var gameProviderId = await _gameProviderService.GetCacheGameProviderIdAsync(GameProviderType.SexyGaming).ConfigureAwait(false);
            var gameProviderCultureCode = await _gameProviderService.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);
            return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "en";
        }

        public async ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode)
        {
            var gameProviderId = await _gameProviderService.GetCacheGameProviderIdAsync(GameProviderType.SexyGaming).ConfigureAwait(false);
            var gameProviderCurrencyCode = await _gameProviderService.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode).ConfigureAwait(false);
            return !string.IsNullOrEmpty(gameProviderCurrencyCode) ? gameProviderCurrencyCode : null;
        }

        public ValueTask<SexyGamingConfiguration> GetSettingConfigurationAsync()
            => _settingService.GetConfigurationAsync<SexyGamingConfiguration>(GameProviderConfigurations.SexyGaming);
    }
}
