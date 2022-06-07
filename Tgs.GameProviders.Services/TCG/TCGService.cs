using System;
using System.Linq;
using System.Threading.Tasks;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.TCG;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.TCG;

namespace Tgs.GameProviders.Services.TCG
{
    internal class TCGService : ITCGService
    {
        private readonly IGameProviderRepository _gameProviderService;
        private readonly IGameService _gameService;
        private readonly ISettingRepository _settingService;

        public TCGService(
            ISettingRepository settingService,
            IGameProviderRepository gameProviderService,
            IGameService gameService
            )
        {
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        }

        public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
        {
            var gameProviderId = await _gameProviderService.GetGameProviderIdAsync(GameProviderType.TCG).ConfigureAwait(false);
            var gameProviderCultureCode = await _gameProviderService.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);
            return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "EN";
        }

        public async ValueTask<TCGMerchant> GetMerchantAsync(string currencyCode)
        {
            var configuration = await GetSettingConfigurationAsync().ConfigureAwait(false);
            return configuration.Merchants.Where(x => x.CurrencyCode == currencyCode).FirstOrDefault();
        }

        public ValueTask<TCGConfiguration> GetSettingConfigurationAsync()
            => _settingService.GetConfigurationAsync<TCGConfiguration>(GameProviderConfigurations.TCG);
    }
}
