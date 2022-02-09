using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Core.LY;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.LY;

namespace TGS.GameProviders.Services.LY
{
    internal class LYService : ILYService
    {
        private readonly IGameProviderService _gameProviderService;
        private readonly ISettingService _settingService;

        public LYService(
            ISettingService settingService,
            IGameProviderService gameProviderService
            )
        {
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
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
            var configuration = await _settingService.GetConfigurationAsync<LYConfiguration>(GameProviderConfigurations.LY).ConfigureAwait(false);
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

        public string GetGameProviderCode()
            => GameProviderType.LY;

        public ValueTask<Guid> GetGameProviderIdAsync()
            => _gameProviderService.GetCacheGameProviderIdAsync(GameProviderType.LY);

        public ValueTask<LYConfiguration> GetSettingConfigurationAsync()
            => _settingService.GetConfigurationAsync<LYConfiguration>(GameProviderConfigurations.LY);
    }
}
