using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.BetConstruct;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.BetConstruct
{
    internal class BetConstructService : IBetConstructService
    {
        private readonly ISettingService _settingService;

        public BetConstructService(ISettingService settingService)
        {
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
        }

        public async ValueTask<BetConstructConfiguration> GetSettingConfigurationAsync() =>
            await _settingService
                .GetConfigurationAsync<BetConstructConfiguration>(GameProviderConfigurations.BetConstruct)
                .ConfigureAwait(false);
    }
}
