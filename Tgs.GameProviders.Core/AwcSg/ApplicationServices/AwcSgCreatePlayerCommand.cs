using Tgs.GameProviders.Domains.ConfigurationData;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.AwcSg.ApplicationServices
{
    public record AwcSgCreatePlayerCommand(
        AwcSgConfiguration Configuration,
        AwcConfigurationData ConfigurationData,
        string PlayerCode,
        string CurrencyCode,
        string Language = "en-US");
}
