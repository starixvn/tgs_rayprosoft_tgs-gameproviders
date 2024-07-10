using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.CQ9Gaming.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.CQ9Gaming.ApplicationServices;

public class CQ9GamingGameUrlGetService(
	IGameProviderRepository gameProvider,
	IPlayerRepository player,
	IBrandRepository brand,
	IGameRepository gameRepository,
	IGameInterfaceCQ9GamingClient client) : IGetService<CQ9GamingGameUrlQuery, Uri>
{
	public async ValueTask<Uri> GetAsync(CQ9GamingGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.CQ9Gaming,
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
		var configuration = await gameProvider.GetCQ9GamingConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await gameProvider.GetGameProviderCultureCodeAsync(
			gameProviderId: gameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en";
		var gameInfo = await gameRepository.GetAsync(
			gameId: query.GameId.Value,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameLoginReply = await client.GameLoginAsync(
			apiUrl: configuration.ApiUrl,
			apiToken: configuration.ApiToken,
			playerCode: playerInfo.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameUrlReply = (gameInfo.Type != GameCommand.Common.GameType.LiveDealerLobby && gameInfo.Type != GameCommand.Common.GameType.LiveDealer)
			? await client.GameUrlAsync(
				apiUrl: configuration.ApiUrl,
				apiToken: configuration.ApiToken,
				userToken: gameLoginReply.UserToken,
				gameCode: gameInfo.GameExternalId,
				isMobile: query.PlatformType == PlatformType.Mobile,
				language: cultureCode,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			: await client.GameUrlAsync(
				apiUrl: configuration.ApiUrl,
				apiToken: configuration.ApiToken,
				userToken: gameLoginReply.UserToken,
				gameCode: "GINKGO01",
				isMobile: query.PlatformType == PlatformType.Mobile,
				language: cultureCode,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new Uri(gameUrlReply.GameUrl);
	}
}
