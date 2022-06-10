using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.AwcSg
{
    public interface IAwcSgGamePlayer
    {
        ValueTask CreatePlayerAsync(AwcSgConfiguration configuration, AwcConfigurationData configurationData, string playerCode, string currencyCode, string language = "en-US", CancellationToken cancellationToken = default);
    }
}
