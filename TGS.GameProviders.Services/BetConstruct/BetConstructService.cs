using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.BetConstruct;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.BetConstruct
{
    internal class BetConstructService : IBetConstructService
    {
        private readonly ISettingRepository _settingRepository;

        public BetConstructService(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
        }

        public async ValueTask<BetConstructConfiguration> GetSettingConfigurationAsync() =>
            await _settingRepository
                .GetConfigurationAsync<BetConstructConfiguration>(GameProviderConfigurations.BetConstruct)
                .ConfigureAwait(false);
    }
}
