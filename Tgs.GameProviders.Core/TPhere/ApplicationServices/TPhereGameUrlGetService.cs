using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.TPhere.Client;

using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.TPhere.ApplicationServices;

public class TPhereGameUrlGetService(
	IGameProviderRepository gameProvider,
	IPlayerRepository player,
	IBrandRepository brand,
	IGameRepository game,
	IGameInterfaceTPhereClient client) : IGetService<TPhereGameUrlQuery, string>
{
	public async ValueTask<string> GetAsync(TPhereGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.TPhere,
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
		var configuration = await gameProvider.GetTPhereConfiguration(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await gameProvider.GetGameProviderCultureCodeAsync(
			gameProviderId: gameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en";
		var gameInfo = await game.GetAsync(
			gameId: query.GameId.Value,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await client.TP2GameUrlAsync(
			apiUrl: configuration.ApiUrl,
			apiKey: configuration.ApiKey,
			apiToken: configuration.ApiToken,
			playerCode: playerInfo.PlayerCode,
			gameCode: gameInfo.GameExternalId,
			isMobile: query.PlatformType == PlatformType.Mobile,
			language: cultureCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!reply.IsPlayerExist)
		{
			await client.TP2PlayerCreateAsync(
				apiUrl: configuration.ApiUrl,
				apiKey: configuration.ApiKey,
				apiToken: configuration.ApiToken,
				playerCode: playerInfo.PlayerCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
			reply = await client.TP2GameUrlAsync(
				apiUrl: configuration.ApiUrl,
				apiKey: configuration.ApiKey,
				apiToken: configuration.ApiToken,
				playerCode: playerInfo.PlayerCode,
				gameCode: gameInfo.GameExternalId,
				isMobile: query.PlatformType == PlatformType.Mobile,
				language: cultureCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}

		return reply.IsPlayerExist
			? reply.GameUrl
			: throw new Exception($"player does not exist.({playerInfo.PlayerCode})");
	}
}
