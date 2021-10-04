using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.TCG;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Core.TCG
{
    internal class TCGService : ITCGService
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly ISettingRepository _settingRepository;

        public TCGService(
            ISettingRepository settingRepository,
            IGameProviderRepository gameProviderRepository
            )
        {
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        }

        public Task<GameProvider> GetGameProviderAsync()
            => _gameProviderRepository.GetGameProviderAsync(GameProviderType.TCG);

        public string GetGameProviderCode()
            => GameProviderType.TCG;

        public async Task<Guid> GetGameProviderIdAsync()
        {
            var gameProvider = await _gameProviderRepository.GetGameProviderAsync(GameProviderType.TCG).ConfigureAwait(false);
            return gameProvider.Id;
        }

        public async ValueTask<TCGMerchant> GetMerchantAsync(string currencyCode)
        {
            var configuration = await _settingRepository.GetConfigurationAsync<TCGConfiguration>(GameProviderConfigurations.TCG).ConfigureAwait(false);
            return configuration.Merchants.Where(x => x.CurrencyCode == currencyCode).FirstOrDefault();
        }

        public async ValueTask<TCGMerchant> GetMerchantByMerchantCodeAsync(string merchantCode)
        {
            var configuration = await _settingRepository.GetConfigurationAsync<TCGConfiguration>(GameProviderConfigurations.TCG).ConfigureAwait(false);
            return configuration.Merchants.Where(x => x.MerchantCode == merchantCode).FirstOrDefault();
        }

        public Task<TCGConfiguration> GetSettingConfigurationAsync()
            => _settingRepository.GetConfigurationAsync<TCGConfiguration>(GameProviderConfigurations.TCG);
    }
}
