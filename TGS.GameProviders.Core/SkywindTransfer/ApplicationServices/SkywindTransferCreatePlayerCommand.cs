using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.SkywindTransfer.ApplicationServices;

public record SkywindTransferCreatePlayerCommand(
    SWTransferConfiguration Configuration,
    string PlayerCode,
    string CurrencyCode);
