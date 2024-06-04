using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.EvolutionTransfer.ApplicationServices;

public class EvolutionTransferPlayerLogoutCommandService : ICommandService<EvolutionTransferPlayerLogoutCommand>
{
	private readonly IGameInterfaceEvolutionClient _grpc;
	private readonly ISettingRepository _setting;

	public EvolutionTransferPlayerLogoutCommandService(
		IGameInterfaceEvolutionClient grpc,
		ISettingRepository setting)
	{
		_grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
		_setting = setting ?? throw new ArgumentNullException(nameof(setting));
	}

	public async ValueTask ExecuteAsync(EvolutionTransferPlayerLogoutCommand command, CancellationToken cancellationToken = default)
	{
		var configuration = await _setting.GetEvolutionTransferConfigurationAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		_ = await _grpc.KickPlayerAsync(
			configuration.ApiUrl,
			configuration.CasinoId,
			configuration.Ua2Token,
			Guid.NewGuid().ToString(),
			command.PlayerCode,
			cancellationToken).ConfigureAwait(false);
	}
}
