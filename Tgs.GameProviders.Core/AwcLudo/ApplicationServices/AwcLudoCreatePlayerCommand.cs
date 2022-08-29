using Tgs.GameProviders.Domains.ConfigurationData;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.AwcLudo.ApplicationServices;

public record AwcLudoCreatePlayerCommand(
    AwcLudoConfiguration Configuration,
    AwcConfigurationData ConfigurationData,
    string PlayerCode,
    string CurrencyCode,
    string Language = "en-US");
