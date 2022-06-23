#nullable enable

using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core
{
    public interface ISettingRepository
    {
        ValueTask<AwcAelottoConfiguration> GetAwcAelottoConfigurationAsync();

        ValueTask<AwcAwsConfiguration> GetAwcAwsConfigurationAsync();

        ValueTask<AwcCockfightConfiguration> GetAwcCockfightConfigurationAsync();

        ValueTask<AwcFacaiConfiguration> GetAwcFacaiConfigurationAsync();

        ValueTask<AwcFsConfiguration> GetAwcFsConfigurationAsync();

        ValueTask<AwcKmConfiguration> GetAwcKmConfigurationAsync();

        ValueTask<AwcLudoConfiguration> GetAwcLudoConfigurationAsync();

        ValueTask<AwcPgsConfiguration> GetAwcPgsConfigurationAsync();

        ValueTask<AwcRcbConfiguration> GetAwcRcbConfigurationAsync();

        ValueTask<AwcSpadeConfiguration> GetAwcSpadeConfigurationAsync();

        ValueTask<AwcSgConfiguration> GetAwcSgConfigurationAsync();

        ValueTask<AwcVenusConfiguration> GetAwcVenusConfigurationAsync();

        ValueTask<AwcYlConfiguration> GetAwcYlConfigurationAsync();

        ValueTask<JiliTransferConfiguration> GetJiliTransferConfigurationAsync();

        ValueTask<WMCasinoTransferConfiguration> GetWMCasinoTransferConfigurationAsync();

        ValueTask<TConfiguration> GetConfigurationAsync<TConfiguration>(string key) where TConfiguration : PluggableConfigurationBase;
    }
}