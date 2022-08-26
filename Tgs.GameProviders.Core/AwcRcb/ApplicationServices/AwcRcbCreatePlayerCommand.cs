using Tgs.GameProviders.Domains.ConfigurationData;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.AwcRcb.ApplicationServices;

public record AwcRcbCreatePlayerCommand(
    AwcRcbConfiguration Configuration,
    AwcConfigurationData ConfigurationData,
    string PlayerCode,
    string CurrencyCode,
    string Language = "en-US");
