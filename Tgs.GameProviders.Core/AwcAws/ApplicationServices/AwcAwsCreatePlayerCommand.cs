using Tgs.GameProviders.Domains.ConfigurationData;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.AwcAws.ApplicationServices;

public record AwcAwsCreatePlayerCommand(
    AwcAwsConfiguration Configuration,
    AwcConfigurationData ConfigurationData,
    string PlayerCode,
    string CurrencyCode,
    string Language = "en-US");
