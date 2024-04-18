using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Kaiyuan.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Kaiyuan.ApplicationServices;

public class KaiyuanPlayerBalanceGetService(
	IGameProviderRepository gameProviderRepository,
	IPlayerRepository playerRepository,
	IBrandRepository brandRepository,
	IGameInterfaceKaiyuanClient client) : IGetService<KaiyuanPlayerBalanceQuery, PlayerBalanceResult>
{
	public async ValueTask<PlayerBalanceResult> GetAsync(KaiyuanPlayerBalanceQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.Kaiyuan,
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
		var configuration = await gameProviderRepository.GetKaiyuanConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await client.PlayerBalanceAsync(
			apiUrl: configuration.ApiUrl,
			deskey: configuration.Deskey,
			md5key: configuration.Md5key,
			agentId: configuration.AgentId,
			playerCode: playerInfo.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!reply.IsPlayerExist)
		{
			_ = await client.GameUrlAsync(
			apiUrl: configuration.ApiUrl,
			deskey: configuration.Deskey,
			md5key: configuration.Md5key,
			agentId: configuration.AgentId,
			brandCode: playerInfo.BrandCode,
			playerCode: playerInfo.PlayerCode,
			ipAddress: "127.0.0.1",
			externalGameId: "0",
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

			reply = await client.PlayerBalanceAsync(
				apiUrl: configuration.ApiUrl,
				deskey: configuration.Deskey,
				md5key: configuration.Md5key,
				agentId: configuration.AgentId,
				playerCode: playerInfo.PlayerCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}

		return reply.IsPlayerExist
			? new PlayerBalanceResult
			{
				FreeBalance = reply.FreeBalance.ToDecimal().GetValueOrDefault(0),
				OutstandingBalance = reply.OutstandingBalance.ToDecimal().GetValueOrDefault(0),
			}
			: throw new Exception($"player does not exist.({playerInfo.PlayerCode})");
	}
}
