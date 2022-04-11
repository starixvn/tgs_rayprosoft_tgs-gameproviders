using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.BetConstruct;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Services.BetConstruct
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
