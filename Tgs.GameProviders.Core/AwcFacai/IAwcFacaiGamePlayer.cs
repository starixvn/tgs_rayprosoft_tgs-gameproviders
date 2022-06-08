using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.AwcFacai
{
    public interface IAwcFacaiGamePlayer
    {
        ValueTask CreatePlayerAsync(AwcFacaiConfiguration configuration, AwcConfigurationData configurationData, string playerCode, string currencyCode, string language = "en-US", CancellationToken cancellationToken = default);
    }
}
