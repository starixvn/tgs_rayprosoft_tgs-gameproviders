using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.SkywindService.Client;
using Tgs.GameInterface.SkywindService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SkywindTransfer.ApplicationServices;

public class SkywindTransferPlayerBalanceGetService(
	ICommandService<SkywindTransferCreatePlayerCommand> createPlayerCommandService,
	IGameInterfaceSkywindClient client,
	IPlayerRepository playerRepository,
	ISettingRepository settingRepository) : IGetService<SkywindTransferPlayerBalanceQuery, PlayerBalanceResult>
{
	public async ValueTask<PlayerBalanceResult> GetAsync(SkywindTransferPlayerBalanceQuery query, CancellationToken cancellationToken = default)
	{
		var configuration = await settingRepository.GetSkywindTransferConfigurationAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var player = await playerRepository.GetAsync(
			query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var request = new PlayerBalanceRequest()
		{
			ApiUrl = configuration.ApiUrl.ToString(),
			SecretKey = configuration.SecretKey,
			Username = configuration.Username,
			Password = configuration.Password,
			PlayerCode = query.PlayerCode
		};

		var reply = await client
				.PlayerBalanceAsync(
					request: request,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

		if (!reply.IsPlayer)
		{
			await createPlayerCommandService.ExecuteAsync(new SkywindTransferCreatePlayerCommand(
				Configuration: configuration,
				PlayerCode: player.PlayerCode,
				CurrencyCode: player.DefaultWalletCurrencyCode),
				cancellationToken).ConfigureAwait(false);

			reply = await client
				.PlayerBalanceAsync(
				request: request,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}

		return !reply.IsPlayer
			? throw new Exception($"Player does not exist.{player.PlayerCode}")
			: new PlayerBalanceResult
			{
				FreeBalance = reply.Balance.ToDecimal().GetValueOrDefault(0)
			};
	}
}
