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

        public async ValueTask<string> GetAgentIdAsync(string brandCode, string currencyCode)
        {
            var brandMerchant = await GetBrandMerchantAsync(brandCode).ConfigureAwait(false);

            return brandMerchant.AgentMappers.Where(x => IgnoreCaseEquals(x.CurrencyCode, currencyCode) && x.Status).FirstOrDefault()?.AgentId;
        }

        public async ValueTask<KYBrandMerchant> GetBrandMerchantAsync(string brandCode)
        {
            var configuration = await _settingService.GetConfigurationAsync<KYConfiguration>(GameProviderConfigurations.KY).ConfigureAwait(false);

            return configuration.BrandMerchants.Where(x => IgnoreCaseEquals(x.BrandCode, brandCode) && x.Status).FirstOrDefault() ??
                configuration.BrandMerchants.Where(x => IgnoreCaseEquals(x.BrandCode, DefConst.BrandCodeDefault) && x.Status).FirstOrDefault(); ;
        }

        public RoundClientStatus GetClientRound(decimal playerWinLoss)
            => playerWinLoss switch
            {
                > 0 => RoundClientStatus.Win,
                < 0 => RoundClientStatus.Loss,
                0 => RoundClientStatus.Draw
            };

        public ValueTask<Guid> GetGameProviderIdAsync()
            => _gameProviderService.GetCacheGameProviderIdAsync(GameProviderType.KY);

        public bool IgnoreCaseEquals(string source, string trigger)
            => string.Equals(source, trigger, StringComparison.OrdinalIgnoreCase);
    }
}
