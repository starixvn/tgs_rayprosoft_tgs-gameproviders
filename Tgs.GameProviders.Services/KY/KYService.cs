using System;
using System.Linq;
using System.Threading.Tasks;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.KY;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.KY;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Services.KY
{
    internal class KYService : IKYService
    {
        private readonly IGameProviderRepository _gameProviderService;
        private readonly ISettingRepository _settingService;

        public KYService(
            ISettingRepository settingService,
            IGameProviderRepository gameProviderService
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

        public GameRoundClientStatus GetClientRound(decimal playerWinLoss)
            => playerWinLoss switch
            {
                > 0 => GameRoundClientStatus.Win,
                < 0 => GameRoundClientStatus.Loss,
                0 => GameRoundClientStatus.Draw
            };

        public ValueTask<Guid> GetGameProviderIdAsync()
            => _gameProviderService.GetGameProviderIdAsync(GameProviderType.KY);

        public bool IgnoreCaseEquals(string source, string trigger)
            => string.Equals(source, trigger, StringComparison.OrdinalIgnoreCase);
    }
}
