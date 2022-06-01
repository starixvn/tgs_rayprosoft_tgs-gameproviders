using System;
using System.Linq;
using System.Threading.Tasks;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.VR;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.VR;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.GameRounds.Common;

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

            return configuration.BrandMerchants.Where(x => IgnoreCaseEquals(x.BrandCode, brandCode) && x.Status).FirstOrDefault() ??
                configuration.BrandMerchants.Where(x => IgnoreCaseEquals(x.BrandCode, DefConst.BrandCodeDefault) && x.Status).FirstOrDefault();
        }

        public GameRoundClientStatus GetClientRound(int state) =>
           state switch
           {
               1 => GameRoundClientStatus.Void,
               2 => GameRoundClientStatus.Loss,
               3 => GameRoundClientStatus.Win,
               _ => GameRoundClientStatus.Open
           };

        public ValueTask<Guid> GetGameProviderIdAsync()
            => _gameProviderService.GetCacheGameProviderIdAsync(GameProviderType.VR);

        public GameRoundStatus GetRoundStatus(int state) =>
           state switch
           {
               1 => GameRoundStatus.Closed,
               2 => GameRoundStatus.Closed,
               3 => GameRoundStatus.Closed,
               _ => GameRoundStatus.Open
           };

        private static bool IgnoreCaseEquals(string source, string trigger)
            => string.Equals(source, trigger, StringComparison.OrdinalIgnoreCase);
    }
}
