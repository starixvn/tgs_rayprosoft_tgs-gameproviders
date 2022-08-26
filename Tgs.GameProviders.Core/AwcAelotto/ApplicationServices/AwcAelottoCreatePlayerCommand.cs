using Tgs.GameProviders.Domains.ConfigurationData;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.AwcAelotto.ApplicationServices;

public record AwcAelottoCreatePlayerCommand(
    AwcAelottoConfiguration Configuration,
    AwcConfigurationData ConfigurationData,
    string PlayerCode,
    string CurrencyCode,
    string Language = "en-US");
