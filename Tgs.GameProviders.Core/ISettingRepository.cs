using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core;

public interface ISettingRepository
{
	ValueTask<TConfiguration> GetConfigurationAsync<TConfiguration>(
		string key,
		CancellationToken cancellationToken = default);
}
