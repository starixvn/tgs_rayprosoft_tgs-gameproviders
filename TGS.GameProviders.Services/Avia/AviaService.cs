using System;
using System.Linq;
using System.Threading.Tasks;
using Tgs.GameProviders.QueryServer.Models;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.Avia;
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.Avia;

namespace TGS.GameProviders.Services.Avia
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
            var configuration = await GetSettingConfigurationAsync().ConfigureAwait(false);
            var brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals(brandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
            }
            return brandMerchant;
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

        public ValueTask<AviaConfiguration> GetSettingConfigurationAsync()
            => _settingService.GetConfigurationAsync<AviaConfiguration>(GameProviderConfigurations.Avia);
    }
}
