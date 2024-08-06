using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.LEGaming.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.LEGaming.ApplicationServices;

public class LEGamingPlayerBalanceGetService(
	IGameProviderRepository gameProviderRepository,
	IPlayerRepository playerRepository,
	IBrandRepository brandRepository,
	IGameInterfaceLEGamingClient client) : IGetService<LEGamingPlayerBalanceQuery, PlayerBalanceResult>
{
	public async ValueTask<PlayerBalanceResult> GetAsync(LEGamingPlayerBalanceQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.LEGaming,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var player = await playerRepository.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameProviderConfigurationId = await brandRepository.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: gameProviderId,
			brandId: player.BrandId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await gameProviderRepository.GetLEGamingConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await client.PlayerBalanceAsync(
			apiUrl: configuration.ApiUrl,
			deskey: configuration.Deskey,
			md5key: configuration.Md5key,
			agentId: configuration.AgentId,
			agentCurrency: configuration.AgentCurrency,
			playerCode: player.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!reply.IsPlayerExist)
		{
			_ = await client.GameUrlAsync(
			apiUrl: configuration.ApiUrl,
			deskey: configuration.Deskey,
			md5key: configuration.Md5key,
			agentId: configuration.AgentId,
			agentCurrency: configuration.AgentCurrency,
			brandCode: player.BrandCode,
			playerCode: player.PlayerCode,
			ipAddress: "127.0.0.1",
			externalGameId: "0",
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

			reply = await client.PlayerBalanceAsync(
			   apiUrl: configuration.ApiUrl,
			   deskey: configuration.Deskey,
			   md5key: configuration.Md5key,
			   agentId: configuration.AgentId,
			   agentCurrency: configuration.AgentCurrency,
			   playerCode: player.PlayerCode,
			   cancellationToken: cancellationToken)
			   .ConfigureAwait(false);
		}

		return reply.IsPlayerExist
			? new PlayerBalanceResult
			{
				FreeBalance = reply.FreeBalance.ToDecimal().GetValueOrDefault(0),
				OutstandingBalance = reply.OutstandingBalance.ToDecimal().GetValueOrDefault(0)
			}
			: throw new Exception($"player does not exist.({player.PlayerCode})");
	}
}
