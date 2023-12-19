using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.BetConstruct;

internal class BetConstructService : IBetConstructService
{
    private readonly ISettingRepository _settingService;

    public BetConstructService(ISettingRepository settingService)
    {
        _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
    }

    public async ValueTask<BetConstructConfiguration> GetSettingConfigurationAsync() =>
        await _settingService
            .GetConfigurationAsync<BetConstructConfiguration>(GameProviderConfigurations.BetConstruct)
            .ConfigureAwait(false);
}