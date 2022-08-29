using Tgs.GameProviders.Domains.ConfigurationData;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.AwcPgs.ApplicationServices;

public record AwcPgsCreatePlayerCommand(
    AwcPgsConfiguration Configuration,
    AwcConfigurationData ConfigurationData,
    string PlayerCode,
    string CurrencyCode,
    string Language = "en-US");
