using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.SkywindTransfer.ApplicationServices;

public record SkywindTransferCreatePlayerCommand(
	SkywindTransferConfiguration Configuration,
	string PlayerCode,
	string CurrencyCode);
