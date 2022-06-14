using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.AwcVenus.ApplicationServices
{
    public record AwcVenusCreatePlayerCommand(
        AwcVenusConfiguration Configuration,
        AwcConfigurationData ConfigurationData,
        string PlayerCode,
        string CurrencyCode,
        string Language = "en-US");
}
