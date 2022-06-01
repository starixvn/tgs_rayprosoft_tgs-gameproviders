using System;
using System.Linq;
using System.Threading.Tasks;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.BigGaming;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.BigGaming;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Services.BigGaming
{
    internal class BigGamingService : IBigGamingService
    {
        private readonly IGameProviderService _gameProviderService;
        private readonly ISettingService _settingService;

        public BigGamingService(
            ISettingService settingService,
            IGameProviderService gameProviderRepository
            )
        {
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            _gameProviderService = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        }

        public async ValueTask<BigGamingBrandMerchant> GetBrandMerchantAsync(string currencyCode)
        {
            var configuration = await _settingService.GetConfigurationAsync<BigGamingConfiguration>(GameProviderConfigurations.BigGaming).ConfigureAwait(false);
            return configuration.BrandMerchants.Where(x => IgnoreCaseEquals(x.CurrencyCode, currencyCode) && x.Status).FirstOrDefault();
        }

        public GameRoundClientStatus GetClientRound(decimal winLoss)
            => winLoss switch
            {
                > 0 => GameRoundClientStatus.Win,
                < 0 => GameRoundClientStatus.Loss,
                0 => GameRoundClientStatus.Draw
            };

        public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
        {
            var gameProviderId = await _gameProviderService.GetCacheGameProviderIdAsync(GameProviderType.BigGaming).ConfigureAwait(false);
            var gameProviderCultureCode = await _gameProviderService.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);
            return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "en_US";
        }

        public ValueTask<Guid> GetGameProviderIdAsync()
            => _gameProviderService.GetCacheGameProviderIdAsync(GameProviderType.BigGaming);

        private static bool IgnoreCaseEquals(string source, string trigger)
            => string.Equals(source, trigger, StringComparison.OrdinalIgnoreCase);
    }
}
