using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.DreamGame;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.DreamGame
{
    internal class DreamGameService : IDreamGameService
    {
        private readonly IGameProviderService _gameProviderService;
        private readonly ISettingService _settingService;

        public DreamGameService(
        IGameProviderService gameProviderService,
        ISettingService settingService)
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

        private ValueTask<Guid> GetGameProviderIdAsync()
            => _gameProviderService.GetGameProviderIdAsync(GameProviderType.DreamGame);
    }
}
