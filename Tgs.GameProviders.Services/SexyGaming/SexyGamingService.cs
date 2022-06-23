using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.SexyGaming;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Services.SexyGaming
{
    internal class SexyGamingService : ISexyGamingService
    {
        private readonly IGameProviderRepository _gameProviderService;
        private readonly ISettingRepository _settingService;

        public SexyGamingService(
            ISettingRepository settingService,
            IGameProviderRepository gameProviderService
            )
        {
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
        }

        public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
        {
            var gameProviderId = await _gameProviderService.GetGameProviderIdAsync(GameProviderType.SexyGaming).ConfigureAwait(false);
            var gameProviderCultureCode = await _gameProviderService.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);
            return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "en";
        }

        public async ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode)
        {
            var gameProviderId = await _gameProviderService.GetGameProviderIdAsync(GameProviderType.SexyGaming).ConfigureAwait(false);
            var gameProviderCurrencyCode = await _gameProviderService.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode).ConfigureAwait(false);
            return !string.IsNullOrEmpty(gameProviderCurrencyCode) ? gameProviderCurrencyCode : null;
        }

        public ValueTask<SexyGamingConfiguration> GetSettingConfigurationAsync()
            => _settingService.GetConfigurationAsync<SexyGamingConfiguration>(GameProviderConfigurations.SexyGaming);

        public bool IsAccountIsNotExists(string status)
            => string.Equals(status, ((int)SexyGamingGameApiError.AccountIsNotExists).ToString("d4"), StringComparison.OrdinalIgnoreCase);

        public bool IsSuccess(string status)
                    => string.Equals(status, ((int)SexyGamingGameApiError.Success).ToString("d4"), StringComparison.OrdinalIgnoreCase);
    }
}
