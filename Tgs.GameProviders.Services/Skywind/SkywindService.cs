using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.Skywind;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Services.Skywind
{
    internal class SkywindService : ISkywindService
    {
        private readonly IGameProviderRepository _gameProviderService;
        private readonly ISettingRepository _settingService;

        public SkywindService(
            ISettingRepository settingService,
            IGameProviderRepository gameProviderService
            )
        {
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
        }

        public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
        {
            var gameProviderId = await _gameProviderService.GetGameProviderIdAsync(GameProviderType.Skywind).ConfigureAwait(false);
            var gameProviderCultureCode = await _gameProviderService.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);
            return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "en";
        }

        public ValueTask<SkywindConfiguration> GetSettingConfigurationAsync()
            => _settingService.GetConfigurationAsync<SkywindConfiguration>(GameProviderConfigurations.Skywind);
    }
}
