using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.JiliTransfer.ApplicationServices;

public record JiliTransferCreatePlayerCommand(
    JiliTransferConfiguration Configuration,
    string PlayerCode);
