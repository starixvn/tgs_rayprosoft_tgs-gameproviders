using Tgs.GameProviders.Domains.ConfigurationData;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.WMCasinoTransfer.ApplicationServices;

public record WMCasinoTransferCreatePlayerCommand(
    WMCasinoTransferConfiguration Configuration,
    string PlayerCode,
    string CurrencyCode,
    string Password = "TgsP@ssword");
