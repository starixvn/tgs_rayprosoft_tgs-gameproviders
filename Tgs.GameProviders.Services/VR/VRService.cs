using System;
using System.Linq;
using System.Threading.Tasks;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.VR;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.VR;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Services.VR
{
    internal class VRService : IVRService
    {
        private readonly IGameProviderService _gameProviderService;
        private readonly ISettingService _settingService;

        public VRService(
            ISettingService settingService,
            IGameProviderService gameProviderService
            )
        {
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
        }

        public async ValueTask<VRBrandMerchant> GetBrandMerchantAsync(string brandCode)
        {
            var configuration = await _settingService.GetConfigurationAsync<VRConfiguration>(GameProviderConfigurations.VR).ConfigureAwait(false);
            var brandMerchant = configuration.BrandMerchants.Where(x => IgnoreCaseEquals(x.BrandCode, brandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(x => IgnoreCaseEquals(x.BrandCode, CustomConst.BrandCodeDefault) && x.Status).FirstOrDefault();
            }
            return brandMerchant;
        }

        public RoundClientStatus GetClientRound(int state) =>
           state switch
           {
               1 => RoundClientStatus.Void,
               2 => RoundClientStatus.Loss,
               3 => RoundClientStatus.Win,
               _ => RoundClientStatus.Open
           };

        public string GetGameProviderCode()
            => GameProviderType.VR;

        public ValueTask<Guid> GetGameProviderIdAsync()
            => _gameProviderService.GetCacheGameProviderIdAsync(GameProviderType.VR);

        public RoundStatus GetRoundStatus(int state) =>
           state switch
           {
               1 => RoundStatus.Closed,
               2 => RoundStatus.Closed,
               3 => RoundStatus.Closed,
               _ => RoundStatus.Open
           };

        public ValueTask<VRConfiguration> GetSettingConfigurationAsync()
            => _settingService.GetConfigurationAsync<VRConfiguration>(GameProviderConfigurations.VR);

        private static bool IgnoreCaseEquals(string source, string trigger)
                                                    => string.Equals(source, trigger, StringComparison.OrdinalIgnoreCase);
    }
}
