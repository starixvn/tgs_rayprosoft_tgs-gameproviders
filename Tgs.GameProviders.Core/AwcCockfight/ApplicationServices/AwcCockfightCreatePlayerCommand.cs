using Tgs.GameProviders.Domains.ConfigurationData;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.AwcCockfight.ApplicationServices
{
    public record AwcCockfightCreatePlayerCommand(
        AwcCockfightConfiguration Configuration,
        AwcConfigurationData ConfigurationData,
        string PlayerCode,
        string CurrencyCode,
        string Language = "en-US");
}
