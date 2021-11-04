using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.VR;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.VR;

namespace TGS.GameProviders.Services.VR
{
    internal class VRService : IVRService
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly ISettingRepository _settingRepository;

        public VRService(
            ISettingRepository settingRepository,
            IGameProviderRepository gameProviderRepository
            )
        {
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        }

        public async ValueTask<VRBrandMerchant> GetBrandMerchantAsync(string brandCode)
        {
            var configuration = await _settingRepository.GetConfigurationAsync<VRConfiguration>(GameProviderConfigurations.VR).ConfigureAwait(false);
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
            => _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.VR);

        public RoundStatus GetRoundStatus(int state) =>
           state switch
           {
               1 => RoundStatus.Closed,
               2 => RoundStatus.Closed,
               3 => RoundStatus.Closed,
               _ => RoundStatus.Open
           };

        public Task<VRConfiguration> GetSettingConfigurationAsync()
            => _settingRepository.GetConfigurationAsync<VRConfiguration>(GameProviderConfigurations.VR);
    }
}
