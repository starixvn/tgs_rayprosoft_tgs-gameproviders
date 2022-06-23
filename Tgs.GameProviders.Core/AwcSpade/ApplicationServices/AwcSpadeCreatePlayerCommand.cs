using Tgs.GameProviders.Domains.ConfigurationData;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.AwcSpade.ApplicationServices
{
    public record AwcSpadeCreatePlayerCommand(
        AwcSpadeConfiguration Configuration,
        AwcConfigurationData ConfigurationData,
        string PlayerCode,
        string CurrencyCode,
        string Language = "en-US");
}
