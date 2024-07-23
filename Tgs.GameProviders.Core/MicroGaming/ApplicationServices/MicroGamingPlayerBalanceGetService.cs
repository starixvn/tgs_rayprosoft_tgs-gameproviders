using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.MicroGaming.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.MicroGaming.ApplicationServices;

public class MicroGamingPlayerBalanceGetService(
	IGameProviderRepository gameProvider,
	IPlayerRepository player,
	IBrandRepository brand,
	IGameInterfaceMicroGamingClient client) : IGetService<MicroGamingPlayerBalanceQuery, PlayerBalanceResult>
{
	public async ValueTask<PlayerBalanceResult> GetAsync(MicroGamingPlayerBalanceQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.MicroGaming,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var playerInfo = await player.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameProviderConfigurationId = await brand.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: gameProviderId,
			brandId: playerInfo.BrandId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await gameProvider.GetMicroGamingConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await client.PlayerBalanceAsync(
			stsApiBaseUrl: configuration.StsApiUrl,
			clientId: configuration.ClientId,
			clientSecret: configuration.ClientSecret,
			apiBaseUrl: configuration.ApiUrl,
			agentCode: configuration.AgentCode,
			playerCode: playerInfo.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!reply.IsPlayerExist)
		{
			await client.PlayerCreateAsync(
				stsApiBaseUrl: configuration.StsApiUrl,
				clientId: configuration.ClientId,
				clientSecret: configuration.ClientSecret,
				apiBaseUrl: configuration.ApiUrl,
				agentCode: configuration.AgentCode,
				playerCode: playerInfo.PlayerCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			reply = await client.PlayerBalanceAsync(
				stsApiBaseUrl: configuration.StsApiUrl,
				clientId: configuration.ClientId,
				clientSecret: configuration.ClientSecret,
				apiBaseUrl: configuration.ApiUrl,
				agentCode: configuration.AgentCode,
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
