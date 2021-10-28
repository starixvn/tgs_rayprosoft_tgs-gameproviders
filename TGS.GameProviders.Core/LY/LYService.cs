using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.Extensions;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.LY;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Core.LY
{
    internal class LYService : ILYService
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly ISettingRepository _settingRepository;

        public LYService(
            ISettingRepository settingRepository,
            IGameProviderRepository gameProviderRepository
            )
        {
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        }

        public async Task<string> GetAgentIdAsync(string brandCode, string currencyCode)
        {
            var configuration = await GetSettingConfigurationAsync().ConfigureAwait(false);
            var brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals(brandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
            }
            return brandMerchant.AgentMappers.Where(x => x.CurrencyCode.IgnoreCaseEquals(currencyCode) && x.Status).FirstOrDefault()?.AgentId;
        }

        public async Task<LYBrandMerchant> GetBrandMerchantAsync(string brandCode)
        {
            var configuration = await _settingRepository.GetConfigurationAsync<LYConfiguration>(GameProviderConfigurations.LY).ConfigureAwait(false);
            var brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals(brandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
            }
            return brandMerchant;
        }

        public ClientRoundStatus GetClientRound(decimal playerWinLoss)
            => playerWinLoss switch
            {
                > 0 => ClientRoundStatus.Win,
                < 0 => ClientRoundStatus.Loss,
                0 => ClientRoundStatus.Draw
            };

        public DateTimeOffset GetDateTimeOffsetTimeZone8() =>
            DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));

        public Task<GameProvider> GetGameProviderAsync()
            => _gameProviderRepository.GetGameProviderAsync(GameProviderType.LY);

        public string GetGameProviderCode()
            => GameProviderType.LY;

        public async Task<Guid> GetGameProviderIdAsync()
        {
            var gameProvider = await _gameProviderRepository.GetGameProviderAsync(GameProviderType.LY).ConfigureAwait(false);
            return gameProvider.Id;
        }

        public Task<LYConfiguration> GetSettingConfigurationAsync()
            => _settingRepository.GetConfigurationAsync<LYConfiguration>(GameProviderConfigurations.LY);
    }
}
