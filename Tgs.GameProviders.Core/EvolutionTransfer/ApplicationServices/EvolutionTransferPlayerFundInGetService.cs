using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.EvolutionTransfer.ApplicationServices;

public class EvolutionTransferPlayerFundInGetService : IGetService<EvolutionTransferPlayerFundInQuery, PlayerFundResult>
{
	private readonly IGameInterfaceEvolutionClient _grpc;
	private readonly IPlayerRepository _player;
	private readonly ISettingRepository _setting;

	public EvolutionTransferPlayerFundInGetService(
		IGameInterfaceEvolutionClient grpc,
		IPlayerRepository player,
		ISettingRepository setting)
	{
		_grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
		_player = player ?? throw new ArgumentNullException(nameof(player));
		_setting = setting ?? throw new ArgumentNullException(nameof(setting));
	}

	public async ValueTask<PlayerFundResult> GetAsync(EvolutionTransferPlayerFundInQuery query, CancellationToken cancellationToken = default)
	{
		var configuration = await _setting.GetEvolutionTransferConfigurationAsync().ConfigureAwait(false);
		var player = await _player.GetAsync(query.PlayerCode).ConfigureAwait(false);

		var reply = await _grpc.FundInAsync(
			configuration.ApiUrl,
			configuration.EcId,
			player.PlayerCode,
			query.OrderId,
			query.Amount,
			cancellationToken).ConfigureAwait(false);

		return new PlayerFundResult(
			OrderId: query.OrderId,
			ExternalOrderId: reply.TransactionId);
	}
}
