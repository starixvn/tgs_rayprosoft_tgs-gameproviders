using Tgs.GameCommand.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Repositories;

internal class SettingRepository(
	IGameCommandClient client) : ISettingRepository
{
	public ValueTask<TConfiguration> GetConfigurationAsync<TConfiguration>(
		string key,
		CancellationToken cancellationToken = default)
		=> GetAsync<TConfiguration>(key, cancellationToken);

	private async ValueTask<TConfiguration> GetAsync<TConfiguration>(
		string key,
		CancellationToken cancellationToken = default)
		=> await client.SettingGetConfigurationAsync<TConfiguration>(
			key: key,
			cancellationToken: cancellationToken)
		.ConfigureAwait(false)
		?? throw new ArgumentNullException(key, "This game product setting does not exist.");
}
