using System;
using System.Linq;
using System.Threading.Tasks;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.Avia;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.Avia;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Services.Avia
{
    internal class AviaService : IAviaService
    {
        private readonly IGameProviderRepository _gameProviderService;
        private readonly ISettingRepository _settingService;

        public AviaService(
            ISettingRepository settingService,
            IGameProviderRepository gameProviderService
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

        public GameRoundClientStatus GetClientRound(string state)
            => state switch
            {
                "None" => GameRoundClientStatus.Open,
                "Cancel" => GameRoundClientStatus.Void,
                "Win" => GameRoundClientStatus.Win,
                "Lose" => GameRoundClientStatus.Loss,
                _ => GameRoundClientStatus.Void
            };

        public string GetGameProviderCode()
            => GameProviderType.Avia;

        public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
        {
            var gameProviderId = await _gameProviderService.GetGameProviderIdAsync(GameProviderType.Avia).ConfigureAwait(false);
            var gameProviderCultureCode = await _gameProviderService.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);
            return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "ENG";
        }

        public async ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode)
        {
            var gameProviderId = await _gameProviderService.GetGameProviderIdAsync(GameProviderType.Avia).ConfigureAwait(false);
            var gameProviderCurrencyCode = await _gameProviderService.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode).ConfigureAwait(false);
            return !string.IsNullOrEmpty(gameProviderCurrencyCode) ? gameProviderCurrencyCode : null;
        }

        public ValueTask<Guid> GetGameProviderIdAsync()
             => _gameProviderService.GetGameProviderIdAsync(GameProviderType.Avia);

        public GameRoundStatus GetRoundStatus(string state)
            => state switch
            {
                "None" => GameRoundStatus.Open,
                _ => GameRoundStatus.Closed
            };

        private static bool IgnoreCaseEquals(string source, string trigger)
            => string.Equals(source, trigger, StringComparison.OrdinalIgnoreCase);
    }
}
