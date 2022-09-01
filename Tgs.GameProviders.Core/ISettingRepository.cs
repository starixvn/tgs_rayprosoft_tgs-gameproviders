#nullable enable

using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core;

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

    ValueTask<AwcSgConfiguration> GetAwcSgConfigurationAsync();

    ValueTask<AwcSpadeConfiguration> GetAwcSpadeConfigurationAsync();

    ValueTask<AwcVenusConfiguration> GetAwcVenusConfigurationAsync();

    ValueTask<AwcYlConfiguration> GetAwcYlConfigurationAsync();

    ValueTask<TConfiguration> GetConfigurationAsync<TConfiguration>(string key) where TConfiguration : PluggableConfigurationBase;

    ValueTask<EvolutionTransferConfiguration> GetEvolutionTransferConfigurationAsync();

    ValueTask<IDNLiveConfiguration> GetIDNLiveConfigurationAsync();

    ValueTask<JiliTransferConfiguration> GetJiliTransferConfigurationAsync();

    ValueTask<SkywindTransferConfiguration> GetSkywindTransferConfigurationAsync();

    ValueTask<WMCasinoTransferConfiguration> GetWMCasinoTransferConfigurationAsync();

    ValueTask<WGSportsConfiguration> GetWGSportsConfigurationAsync();
}
