using System;
using System.Linq;
using System.Threading.Tasks;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.KY;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.KY;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Services.KY
{
    internal class KYService : IKYService
    {
        private readonly IGameProviderService _gameProviderService;
        private readonly ISettingService _settingService;

        public KYService(
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
            var brandMerchant = configuration.BrandMerchants.Where(x => IgnoreCaseEquals(x.BrandCode, brandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(x => IgnoreCaseEquals(x.BrandCode, CustomConst.BrandCodeDefault) && x.Status).FirstOrDefault();
            }
            return brandMerchant.AgentMappers.Where(x => IgnoreCaseEquals(x.CurrencyCode, currencyCode) && x.Status).FirstOrDefault()?.AgentId;
        }

        public async Task<KYBrandMerchant> GetBrandMerchantAsync(string brandCode)
        {
            var configuration = await _settingService.GetConfigurationAsync<KYConfiguration>(GameProviderConfigurations.KY).ConfigureAwait(false);
            var brandMerchant = configuration.BrandMerchants.Where(x => IgnoreCaseEquals(x.BrandCode, brandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(x => IgnoreCaseEquals(x.BrandCode, CustomConst.BrandCodeDefault) && x.Status).FirstOrDefault();
            }
            return brandMerchant;
        }

        public RoundClientStatus GetClientRound(decimal playerWinLoss)
            => playerWinLoss switch
            {
                > 0 => RoundClientStatus.Win,
                < 0 => RoundClientStatus.Loss,
                0 => RoundClientStatus.Draw
            };

        public DateTimeOffset GetDateTimeOffsetTimeZone8() =>
            DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));

        public string GetGameProviderCode()
            => GameProviderType.KY;

        public ValueTask<Guid> GetGameProviderIdAsync()
            => _gameProviderService.GetCacheGameProviderIdAsync(GameProviderType.KY);

        public ValueTask<KYConfiguration> GetSettingConfigurationAsync()
            => _settingService.GetConfigurationAsync<KYConfiguration>(GameProviderConfigurations.KY);

        private static bool IgnoreCaseEquals(string source, string trigger)
                                                            => string.Equals(source, trigger, StringComparison.OrdinalIgnoreCase);
    }
}
