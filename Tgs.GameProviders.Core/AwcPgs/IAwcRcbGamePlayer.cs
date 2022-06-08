using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.AwcPgs
{
    public interface IAwcRcbGamePlayer
    {
        ValueTask CreatePlayerAsync(AwcRcbConfiguration configuration, AwcConfigurationData configurationData, string playerCode, string currencyCode, string language = "en-US", CancellationToken cancellationToken = default);
    }
}
