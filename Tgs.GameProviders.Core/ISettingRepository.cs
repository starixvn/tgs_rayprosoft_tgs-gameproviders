using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core;

public interface ISettingRepository
{
    ValueTask<AwcAelottoConfiguration> GetAwcAelottoConfigurationAsync(
        CancellationToken cancellationToken = default);

    ValueTask<AwcAwsConfiguration> GetAwcAwsConfigurationAsync(
        CancellationToken cancellationToken = default);

    ValueTask<AwcCockfightConfiguration> GetAwcCockfightConfigurationAsync(
         CancellationToken cancellationToken = default);

    ValueTask<AwcFacaiConfiguration> GetAwcFacaiConfigurationAsync(
        CancellationToken cancellationToken = default);

    ValueTask<AwcFsConfiguration> GetAwcFsConfigurationAsync(
        CancellationToken cancellationToken = default);

    ValueTask<AwcKmConfiguration> GetAwcKmConfigurationAsync(
        CancellationToken cancellationToken = default);

    ValueTask<AwcLudoConfiguration> GetAwcLudoConfigurationAsync(
        CancellationToken cancellationToken = default);

    ValueTask<AwcPgsConfiguration> GetAwcPgsConfigurationAsync(
        CancellationToken cancellationToken = default);

    ValueTask<AwcRcbConfiguration> GetAwcRcbConfigurationAsync(
        CancellationToken cancellationToken = default);

    ValueTask<AwcSgConfiguration> GetAwcSgConfigurationAsync(
        CancellationToken cancellationToken = default);

    ValueTask<AwcSpadeConfiguration> GetAwcSpadeConfigurationAsync(
        CancellationToken cancellationToken = default);

    ValueTask<AwcVenusConfiguration> GetAwcVenusConfigurationAsync(
        CancellationToken cancellationToken = default);

    ValueTask<AwcYlConfiguration> GetAwcYlConfigurationAsync(
        CancellationToken cancellationToken = default);

    ValueTask<TConfiguration> GetConfigurationAsync<TConfiguration>(
        string key,
        CancellationToken cancellationToken = default) where TConfiguration : PluggableConfigurationBase;

    ValueTask<EvolutionTransferConfiguration> GetEvolutionTransferConfigurationAsync(
        CancellationToken cancellationToken = default);

    ValueTask<IDNLiveConfiguration> GetIDNLiveConfigurationAsync(
        CancellationToken cancellationToken = default);

    ValueTask<SkywindTransferConfiguration> GetSkywindTransferConfigurationAsync(
        CancellationToken cancellationToken = default);
}
