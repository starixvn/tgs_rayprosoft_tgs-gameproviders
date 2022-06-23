using Tgs.GameProviders.Domains.ConfigurationData;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.AwcFs.ApplicationServices
{
    public record AwcFsCreatePlayerCommand(
        AwcFsConfiguration Configuration,
        AwcConfigurationData ConfigurationData,
        string PlayerCode,
        string CurrencyCode,
        string Language = "en-US");
}
