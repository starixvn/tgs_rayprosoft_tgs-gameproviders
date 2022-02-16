using System;
using System.Linq;
using System.Threading.Tasks;
using Tgs.GameProviders.QueryServer.Models;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Core.KY;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.KY;

namespace TGS.GameProviders.Services.KY
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
            var brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals(brandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
            }
            return brandMerchant.AgentMappers.Where(x => x.CurrencyCode.IgnoreCaseEquals(currencyCode) && x.Status).FirstOrDefault()?.AgentId;
        }

        public async Task<KYBrandMerchant> GetBrandMerchantAsync(string brandCode)
        {
            var configuration = await _settingService.GetConfigurationAsync<KYConfiguration>(GameProviderConfigurations.KY).ConfigureAwait(false);
            var brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals(brandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
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
    }
}
