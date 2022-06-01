using System;
using System.Linq;
using System.Threading.Tasks;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.LY;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.LY;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Services.LY
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

        public async ValueTask<string> GetAgentIdAsync(string brandCode, string currencyCode)
        {
            var brandMerchant = await GetBrandMerchantAsync(brandCode).ConfigureAwait(false);

            return brandMerchant.AgentMappers.Where(x => IgnoreCaseEquals(x.CurrencyCode, currencyCode) && x.Status).FirstOrDefault()?.AgentId;
        }

        public async ValueTask<LYBrandMerchant> GetBrandMerchantAsync(string brandCode)
        {
            var configuration = await _settingService.GetConfigurationAsync<LYConfiguration>(GameProviderConfigurations.LY).ConfigureAwait(false);

            return configuration.BrandMerchants.Where(x => IgnoreCaseEquals(x.BrandCode, brandCode) && x.Status).FirstOrDefault() ??
                configuration.BrandMerchants.Where(x => IgnoreCaseEquals(x.BrandCode, DefConst.BrandCodeDefault) && x.Status).FirstOrDefault();
        }

        public GameRoundClientStatus GetClientRound(decimal playerWinLoss)
            => playerWinLoss switch
            {
                > 0 => GameRoundClientStatus.Win,
                < 0 => GameRoundClientStatus.Loss,
                0 => GameRoundClientStatus.Draw
            };

        public ValueTask<Guid> GetGameProviderIdAsync()
            => _gameProviderService.GetCacheGameProviderIdAsync(GameProviderType.LY);

        public bool IgnoreCaseEquals(string source, string trigger)
            => string.Equals(source, trigger, StringComparison.OrdinalIgnoreCase);
    }
}
