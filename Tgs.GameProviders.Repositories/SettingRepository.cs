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

    public ValueTask<TConfiguration> GetConfigurationAsync<TConfiguration>(
        string key,
        CancellationToken cancellationToken = default) where TConfiguration : PluggableConfigurationBase
        => GetAsync<TConfiguration>(key, cancellationToken);

    public ValueTask<EvolutionTransferConfiguration> GetEvolutionTransferConfigurationAsync(
        CancellationToken cancellationToken = default)
        => GetAsync<EvolutionTransferConfiguration>(GameProviderConfigurations.EvolutionTransfer, cancellationToken);

    public ValueTask<SkywindTransferConfiguration> GetSkywindTransferConfigurationAsync(
        CancellationToken cancellationToken = default)
        => GetAsync<SkywindTransferConfiguration>(GameProviderConfigurations.SkywindTransfer, cancellationToken);

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
