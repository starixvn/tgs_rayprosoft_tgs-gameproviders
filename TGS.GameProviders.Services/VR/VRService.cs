using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Core.VR;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.VR;

namespace TGS.GameProviders.Services.VR
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
            var brandMerchant = configuration.BrandMerchants
                 .Where(x => x.BrandCode.IgnoreCaseEquals(brandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants
                    .Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
            }
            return brandMerchant;
        }

        public ClientRoundStatus GetClientRound(int state) =>
           state switch
           {
               1 => ClientRoundStatus.Void,
               2 => ClientRoundStatus.Loss,
               3 => ClientRoundStatus.Win,
               _ => ClientRoundStatus.Open
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
    }
}
