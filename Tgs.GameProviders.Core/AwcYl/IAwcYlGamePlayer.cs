using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.AwcYl
{
    public interface IAwcYlGamePlayer
    {
        ValueTask CreatePlayerAsync(AwcYlConfiguration configuration, AwcConfigurationData configurationData, string playerCode, string currencyCode, string language = "en-US", CancellationToken cancellationToken = default);
    }
}
