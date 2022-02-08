using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.BigGaming;
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.BigGaming;

namespace TGS.GameProviders.Services.BigGaming
{
    internal class BigGamingService : IBigGamingService
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly ISettingService _settingService;

        public BigGamingService(
            ISettingService settingService,
            IGameProviderRepository gameProviderRepository
            )
        {
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        }

        public async ValueTask<BigGamingBrandMerchant> GetBrandMerchantAsync(string currencyCode)
        {
            var configuration = await _settingService.GetConfigurationAsync<BigGamingConfiguration>(GameProviderConfigurations.BigGaming).ConfigureAwait(false);
            return configuration.BrandMerchants.Where(x => x.CurrencyCode.IgnoreCaseEquals(currencyCode) && x.Status).FirstOrDefault();
        }

        public ClientRoundStatus GetClientRound(decimal winLoss)
            => winLoss switch
            {
                > 0 => ClientRoundStatus.Win,
                < 0 => ClientRoundStatus.Loss,
                0 => ClientRoundStatus.Draw
            };

        public string GetGameProviderCode()
            => GameProviderType.BigGaming;

        public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
        {
            var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.Skywind).ConfigureAwait(false);
            var (IsCultureCode, GameProviderCultureCode) = await _gameProviderRepository.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);
            return (IsCultureCode) ? GameProviderCultureCode : "en_US";
        }

        public ValueTask<Guid> GetGameProviderIdAsync()
            => _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.BigGaming);

        public ValueTask<BigGamingConfiguration> GetSettingConfigurationAsync()
            => _settingService.GetConfigurationAsync<BigGamingConfiguration>(GameProviderConfigurations.BigGaming);
    }
}
