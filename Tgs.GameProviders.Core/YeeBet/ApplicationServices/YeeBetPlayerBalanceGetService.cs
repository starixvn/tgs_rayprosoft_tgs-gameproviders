using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.YeeBet.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.YeeBet.ApplicationServices;

public class YeeBetPlayerBalanceGetService(
	IGameProviderRepository gameProviderRepository,
	IPlayerRepository playerRepository,
	IBrandRepository brandRepository,
	IGameInterfaceYeeBetClient client,
	ICommandService<YeeBetPlayerCreateCommand> playerCreateService) : IGetService<YeeBetPlayerBalanceQuery, PlayerBalanceResult>
{
	public async ValueTask<PlayerBalanceResult> GetAsync(YeeBetPlayerBalanceQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.YeeBet,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var playerInfo = await playerRepository.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameProviderConfigurationId = await brandRepository.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: gameProviderId,
			brandId: playerInfo.BrandId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await gameProviderRepository.GetYeeBetConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await client.PlayerBalanceAsync(
			apiUrl: configuration.ApiUrl,
			appId: configuration.AppId,
			secretKey: configuration.SecretKey,
			playerCode: playerInfo.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!reply.IsPlayerExist)
		{
			await playerCreateService
				.ExecuteAsync(
				command: new YeeBetPlayerCreateCommand(
					playerInfo.PlayerCode),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			reply = await client.PlayerBalanceAsync(
				apiUrl: configuration.ApiUrl,
				appId: configuration.AppId,
				secretKey: configuration.SecretKey,
				playerCode: playerInfo.PlayerCode,
				cancellationToken: cancellationToken)
			   .ConfigureAwait(false);
		}

		return !reply.IsPlayerExist
			? throw new Exception($"Player does not exist.{playerInfo.PlayerCode}")
			: new PlayerBalanceResult
			{
				FreeBalance = reply.Balance.ToDecimal().GetValueOrDefault(0)
			};
	}
}
