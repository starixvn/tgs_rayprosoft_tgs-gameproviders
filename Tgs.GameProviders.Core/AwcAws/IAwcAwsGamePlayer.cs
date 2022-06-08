using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.AwcAws
{
    public interface IAwcAwsGamePlayer
    {
        ValueTask CreatePlayerAsync(AwcAwsConfiguration configuration, AwcConfigurationData configurationData, string playerCode, string currencyCode, string language = "en-US", CancellationToken cancellationToken = default);
    }
}
