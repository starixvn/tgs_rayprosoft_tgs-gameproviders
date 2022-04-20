using System;
using System.Linq;
using System.Threading.Tasks;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.Avia;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.Avia;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Services.Avia
{
    internal class AviaService : IAviaService
    {
        private readonly IGameProviderService _gameProviderService;
        private readonly ISettingService _settingService;

        public AviaService(
            ISettingService settingService,
            IGameProviderService gameProviderService
            )
        {
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
        }

        public async ValueTask<AviaBrandMerchant> GetBrandMerchantAsync(string brandCode)
        {
            var configuration = await _settingService.GetConfigurationAsync<AviaConfiguration>(GameProviderConfigurations.Avia).ConfigureAwait(false);

            return configuration.BrandMerchants.Where(x => IgnoreCaseEquals(x.BrandCode, brandCode) && x.Status).FirstOrDefault() ??
                configuration.BrandMerchants.Where(x => IgnoreCaseEquals(x.BrandCode, DefConst.BrandCodeDefault) && x.Status).FirstOrDefault();
        }

        public RoundClientStatus GetClientRound(string state)
            => state switch
            {
                "None" => RoundClientStatus.Open,
                "Cancel" => RoundClientStatus.Void,
                "Win" => RoundClientStatus.Win,
                "Lose" => RoundClientStatus.Loss,
                _ => RoundClientStatus.Void
            };

        public string GetGameProviderCode()
            => GameProviderType.Avia;

        public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
        {
            var gameProviderId = await _gameProviderService.GetCacheGameProviderIdAsync(GameProviderType.Avia).ConfigureAwait(false);
            var gameProviderCultureCode = await _gameProviderService.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);
            return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "ENG";
        }

        public async ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode)
        {
            var gameProviderId = await _gameProviderService.GetCacheGameProviderIdAsync(GameProviderType.Avia).ConfigureAwait(false);
            var gameProviderCurrencyCode = await _gameProviderService.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode).ConfigureAwait(false);
            return !string.IsNullOrEmpty(gameProviderCurrencyCode) ? gameProviderCurrencyCode : null;
        }

        public ValueTask<Guid> GetGameProviderIdAsync()
             => _gameProviderService.GetCacheGameProviderIdAsync(GameProviderType.Avia);

        public RoundStatus GetRoundStatus(string state)
            => state switch
            {
                "None" => RoundStatus.Open,
                _ => RoundStatus.Closed
            };

        private static bool IgnoreCaseEquals(string source, string trigger)
            => string.Equals(source, trigger, StringComparison.OrdinalIgnoreCase);
    }
}
