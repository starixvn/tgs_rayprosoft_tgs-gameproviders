using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.EvolutionTransfer.ApplicationServices;

public class EvolutionTransferPlayerBalanceGetService : IGetService<EvolutionTransferPlayerBalanceQuery, PlayerBalanceResult>
{
	private readonly IGameProviderRepository _gameProvider;
	private readonly IGameInterfaceEvolutionClient _grpc;
	private readonly IPlayerRepository _player;
	private readonly ISettingRepository _setting;

	public EvolutionTransferPlayerBalanceGetService(
		IGameProviderRepository gameProvider,
		IGameInterfaceEvolutionClient grpc,
		IPlayerRepository player,
		ISettingRepository setting)
	{
		_gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
		_grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
		_player = player ?? throw new ArgumentNullException(nameof(player));
		_setting = setting ?? throw new ArgumentNullException(nameof(setting));
	}

	public async ValueTask<PlayerBalanceResult> GetAsync(EvolutionTransferPlayerBalanceQuery command, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.EvolutionTransfer).ConfigureAwait(false);
		var configuration = await _setting.GetEvolutionTransferConfigurationAsync().ConfigureAwait(false);
		var player = await _player.GetAsync(command.PlayerCode).ConfigureAwait(false);

		var reply = await _grpc.GetBalanceAsync(
			configuration.ApiUrl,
			configuration.EcId,
			player.PlayerCode,
			cancellationToken).ConfigureAwait(false);

		if (!reply.IsPlayerExist)
		{
			var currencyCode = await _gameProvider
				.GetGameProviderCurrencyCodeAsync(gameProviderId, player.DefaultWalletCurrencyCode)
				.ConfigureAwait(false)
				?? throw new Exception($"This game product currency does not exist.({player.DefaultWalletCurrencyCode})");

			_ = await _grpc.GetLiveDealerGameUrlAsync(
				ua2AuthenticationUrl: configuration.Ua2AuthenticationUrl,
				brandCode: player.BrandCode,
				playerCode: player.PlayerCode,
				platformType: (short)PlatformType.Desktop,
				ipAddress: "127.0.0.1",
				encryptedToken: Guid.NewGuid().ToString(),
				externalCultureCode: "cn",
				externalCurrencyCode: currencyCode,
				cancellationToken: cancellationToken).ConfigureAwait(false);

			reply = await _grpc
				.GetBalanceAsync(
				apiUrl: configuration.ApiUrl,
				agentId: configuration.EcId,
				playerCode: player.PlayerCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}

		return !reply.IsPlayerExist
			? throw new Exception($"Player does not exist.{player.PlayerCode}")
			: new PlayerBalanceResult
			{
				FreeBalance = reply.Balance.ToDecimal().GetValueOrDefault(0)
			};
	}
}
