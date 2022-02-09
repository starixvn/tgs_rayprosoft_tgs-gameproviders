using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.Skywind;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.Skywind
{
    internal class SkywindService : ISkywindService
    {
        private readonly IGameProviderService _gameProviderService;
        private readonly ISettingService _settingService;

        public SkywindService(
            ISettingService settingService,
            IGameProviderService gameProviderService
            )
        {
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
        }

        public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
        {
            var gameProviderId = await _gameProviderService.GetCacheGameProviderIdAsync(GameProviderType.Skywind).ConfigureAwait(false);
            var gameProviderCultureCode = await _gameProviderService.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);
            return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "en";
        }


        public ValueTask<SkywindConfiguration> GetSettingConfigurationAsync()
            => _settingService.GetConfigurationAsync<SkywindConfiguration>(GameProviderConfigurations.Skywind);
    }
}
