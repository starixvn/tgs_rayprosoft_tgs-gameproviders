using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Jili.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.JiliTransfer.ApplicationServices;

public class JiliTransferGameUrlGetService(
	IBrandRepository brandRepository,
	IGameRepository gameRepository,
	IGameProviderRepository gameProviderRepository,
	IGameInterfaceJiliClient client,
	IPlayerRepository playerRepository) : IGetService<JiliTransferGameUrlQuery, string>
{
	public async ValueTask<string> GetAsync(JiliTransferGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.JiliTransfer,
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
		var configuration = await gameProviderRepository.GetJiliTransferConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await gameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: gameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en-US";
		var gameInfo = await gameRepository.GetAsync(
			gameId: query.GameId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await client.GameUrlAsync(
			apiUrl: configuration.ApiUrl,
			agentId: configuration.AgentId,
			agentKey: configuration.AgentKey,
			playerCode: playerInfo.PlayerCode,
			externalGameId: gameInfo.GameExternalId,
			language: cultureCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!reply.IsPlayerExist)
		{
			await client.PlayerCreateAsync(
				apiUrl: configuration.ApiUrl,
				agentId: configuration.AgentId,
				agentKey: configuration.AgentKey,
				playerCode: playerInfo.PlayerCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			reply = await client.GameUrlAsync(
				apiUrl: configuration.ApiUrl,
				agentId: configuration.AgentId,
				agentKey: configuration.AgentKey,
				playerCode: playerInfo.PlayerCode,
				externalGameId: gameInfo.GameExternalId,
				language: cultureCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}

		return !reply.IsPlayerExist
			? throw new Exception($"Player does not exist.({playerInfo.PlayerCode})")
			: reply.GameUrl;
	}
}
