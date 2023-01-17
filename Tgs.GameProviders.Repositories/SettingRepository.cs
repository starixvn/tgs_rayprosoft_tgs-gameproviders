using Tgs.GameCommand.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Repositories;

internal class SettingRepository : ISettingRepository
{
    private readonly IGameCommandClient _client;

    public SettingRepository(
        IGameCommandClient client
        )
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public ValueTask<AwcAelottoConfiguration> GetAwcAelottoConfigurationAsync(
        CancellationToken cancellationToken = default)
        => GetAsync<AwcAelottoConfiguration>(GameProviderConfigurations.AWC_AELOTTO, cancellationToken);

    public ValueTask<AwcAwsConfiguration> GetAwcAwsConfigurationAsync(
        CancellationToken cancellationToken = default)
        => GetAsync<AwcAwsConfiguration>(GameProviderConfigurations.AWC_AWS, cancellationToken);

    public ValueTask<AwcCockfightConfiguration> GetAwcCockfightConfigurationAsync(
        CancellationToken cancellationToken = default)
        => GetAsync<AwcCockfightConfiguration>(GameProviderConfigurations.AWC_COCKFIGHT, cancellationToken);

    public ValueTask<AwcFacaiConfiguration> GetAwcFacaiConfigurationAsync(
        CancellationToken cancellationToken = default)
        => GetAsync<AwcFacaiConfiguration>(GameProviderConfigurations.AWC_FACAI, cancellationToken);

    public ValueTask<AwcFsConfiguration> GetAwcFsConfigurationAsync(
        CancellationToken cancellationToken = default)
        => GetAsync<AwcFsConfiguration>(GameProviderConfigurations.AWC_FS, cancellationToken);

    public ValueTask<AwcKmConfiguration> GetAwcKmConfigurationAsync(
        CancellationToken cancellationToken = default)
        => GetAsync<AwcKmConfiguration>(GameProviderConfigurations.AWC_KM, cancellationToken);

    public ValueTask<AwcLudoConfiguration> GetAwcLudoConfigurationAsync(
        CancellationToken cancellationToken = default)
        => GetAsync<AwcLudoConfiguration>(GameProviderConfigurations.AWC_LUDO, cancellationToken);

    public ValueTask<AwcPgsConfiguration> GetAwcPgsConfigurationAsync(
        CancellationToken cancellationToken = default)
        => GetAsync<AwcPgsConfiguration>(GameProviderConfigurations.AWC_PGS, cancellationToken);

    public ValueTask<AwcRcbConfiguration> GetAwcRcbConfigurationAsync(
        CancellationToken cancellationToken = default)
        => GetAsync<AwcRcbConfiguration>(GameProviderConfigurations.AWC_RCB, cancellationToken);

    public ValueTask<AwcSgConfiguration> GetAwcSgConfigurationAsync(
        CancellationToken cancellationToken = default)
       => GetAsync<AwcSgConfiguration>(GameProviderConfigurations.AWC_SG, cancellationToken);

    public ValueTask<AwcSpadeConfiguration> GetAwcSpadeConfigurationAsync(
        CancellationToken cancellationToken = default)
        => GetAsync<AwcSpadeConfiguration>(GameProviderConfigurations.AWC_SPADE, cancellationToken);

    public ValueTask<AwcVenusConfiguration> GetAwcVenusConfigurationAsync(
        CancellationToken cancellationToken = default)
        => GetAsync<AwcVenusConfiguration>(GameProviderConfigurations.AWC_VENUS, cancellationToken);

    public ValueTask<AwcYlConfiguration> GetAwcYlConfigurationAsync(
        CancellationToken cancellationToken = default)
        => GetAsync<AwcYlConfiguration>(GameProviderConfigurations.AWC_YL, cancellationToken);

    public ValueTask<TConfiguration> GetConfigurationAsync<TConfiguration>(
        string key,
        CancellationToken cancellationToken = default) where TConfiguration : PluggableConfigurationBase
        => GetAsync<TConfiguration>(key, cancellationToken);

    public ValueTask<EvolutionTransferConfiguration> GetEvolutionTransferConfigurationAsync(
        CancellationToken cancellationToken = default)
        => GetAsync<EvolutionTransferConfiguration>(GameProviderConfigurations.EvolutionTransfer, cancellationToken);

    public ValueTask<IDNLiveConfiguration> GetIDNLiveConfigurationAsync(
        CancellationToken cancellationToken = default)
        => GetAsync<IDNLiveConfiguration>(GameProviderConfigurations.IDNLive, cancellationToken);

    public ValueTask<SkywindTransferConfiguration> GetSkywindTransferConfigurationAsync(
        CancellationToken cancellationToken = default)
        => GetAsync<SkywindTransferConfiguration>(GameProviderConfigurations.SkywindTransfer, cancellationToken);

    public ValueTask<WGFmGamingConfiguration> GetWGSportsConfigurationAsync(
        CancellationToken cancellationToken = default)
        => GetAsync<WGFmGamingConfiguration>(GameProviderConfigurations.WGSports, cancellationToken);

    private async ValueTask<TConfiguration> GetAsync<TConfiguration>(
        string key,
        CancellationToken cancellationToken = default)
        => await _client
        .SettingGetConfigurationAsync<TConfiguration>(
            key: key,
            cancellationToken: cancellationToken)
        .ConfigureAwait(false)
        ?? throw new Exception($"This game product setting does not exist.({key})");
}
