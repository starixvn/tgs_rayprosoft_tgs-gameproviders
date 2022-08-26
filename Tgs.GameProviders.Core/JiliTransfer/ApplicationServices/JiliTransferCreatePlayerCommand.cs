using Tgs.GameProviders.Domains.ConfigurationData;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.JiliTransfer.ApplicationServices;

public record JiliTransferCreatePlayerCommand(
    JiliTransferConfiguration Configuration,
    JiliTransferConfigurationData ConfigurationData,
    string PlayerCode);
