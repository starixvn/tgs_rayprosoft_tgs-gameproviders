using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Avia;
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.Avia;

namespace TGS.GameProviders.Services.Avia
{
    internal class AviaService : IAviaService
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly ISettingRepository _settingRepository;

        public AviaService(
            ISettingRepository settingRepository,
            IGameProviderRepository gameProviderRepository
            )
        {
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
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

        public ClientRoundStatus GetClientRound(string state)
            => state switch
            {
                "None" => ClientRoundStatus.Open,
                "Cancel" => ClientRoundStatus.Void,
                "Win" => ClientRoundStatus.Win,
                "Lose" => ClientRoundStatus.Loss,
                _ => ClientRoundStatus.Void
            };

        public string GetGameProviderCode()
            => GameProviderType.Avia;

        public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
        {
            var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.Avia).ConfigureAwait(false);
            var (IsCultureCode, GameProviderCultureCode) = await _gameProviderRepository.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);
            return (IsCultureCode) ? GameProviderCultureCode : "ENG";
        }

        public async ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode)
        {
            var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.Avia).ConfigureAwait(false);
            var (IsCurrencyCode, GameProviderCurrencyCode) = await _gameProviderRepository.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode).ConfigureAwait(false);
            return (IsCurrencyCode) ? GameProviderCurrencyCode : null;
        }

        public ValueTask<Guid> GetGameProviderIdAsync()
             => _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.Avia);

        public RoundStatus GetRoundStatus(string state)
            => state switch
            {
                "None" => RoundStatus.Open,
                _ => RoundStatus.Closed
            };

        public Task<AviaConfiguration> GetSettingConfigurationAsync()
            => _settingRepository.GetConfigurationAsync<AviaConfiguration>(GameProviderConfigurations.Avia);
    }
}
