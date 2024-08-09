using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.GlobalTechnology.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.GTPlaytech.ApplicationServices;

public class GTPlaytechGameUrlGetService(
	IGameProviderRepository gameProvider,
	IPlayerRepository player,
	IBrandRepository brand,
	IGameRepository gameRepository,
	IGameInterfaceGlobalTechnologyClient client) : IGetService<GTPlaytechGameUrlQuery, string>
{
	public async ValueTask<string> GetAsync(GTPlaytechGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.GT_Playtech,
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
		var configuration = await gameProvider.GetGTPlaytechConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var currencyCode = await gameProvider.GetGameProviderCurrencyCodeAsync(
			gameProviderId: gameProviderId,
			currencyCode: playerInfo.DefaultWalletCurrencyCode,
			cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await gameProvider.GetGameProviderCultureCodeAsync(
			gameProviderId: gameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "EN";
		var gameInfo = await gameRepository.GetAsync(
			gameId: query.GameId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = gameInfo.Type is GameCommand.Common.GameType.LiveDealerLobby or GameCommand.Common.GameType.LiveDealer
			? await client.GameUrlPlaytechByLiveAsync(
				apiUrl: configuration.ApiUrl,
				launcherApiUrl: configuration.LauncherApiUrl,
				brandId: configuration.BrandId,
				secretKey: configuration.SecretKey,
				playerCode: playerInfo.PlayerCode,
				currency: currencyCode,
				language: cultureCode,
				isMobile: query.PlatformType == PlatformType.Mobile,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			: await client.GameUrlAsync(
				apiUrl: configuration.ApiUrl,
				launcherApiUrl: configuration.LauncherApiUrl,
				brandId: configuration.BrandId,
				secretKey: configuration.SecretKey,
				playerCode: playerInfo.PlayerCode,
				currency: currencyCode,
				language: cultureCode,
				isMobile: query.PlatformType == PlatformType.Mobile,
				gameCode: gameInfo.GameExternalId,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!reply.IsPlayerExist)
		{
			await client.PlayerCreateAsync(
				apiUrl: configuration.ApiUrl,
				brandId: configuration.BrandId,
				secretKey: configuration.SecretKey,
				playerCode: playerInfo.PlayerCode,
				currency: currencyCode,
				language: cultureCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			reply = gameInfo.Type is GameCommand.Common.GameType.LiveDealerLobby or GameCommand.Common.GameType.LiveDealer
				? await client.GameUrlPlaytechByLiveAsync(
				   apiUrl: configuration.ApiUrl,
				   launcherApiUrl: configuration.LauncherApiUrl,
				   brandId: configuration.BrandId,
				   secretKey: configuration.SecretKey,
				   playerCode: playerInfo.PlayerCode,
				   currency: currencyCode,
				   language: cultureCode,
				   isMobile: query.PlatformType == PlatformType.Mobile,
				   cancellationToken: cancellationToken)
			   .ConfigureAwait(false)
			   : await client.GameUrlAsync(
				   apiUrl: configuration.ApiUrl,
				   launcherApiUrl: configuration.LauncherApiUrl,
				   brandId: configuration.BrandId,
				   secretKey: configuration.SecretKey,
				   playerCode: playerInfo.PlayerCode,
				   currency: currencyCode,
				   language: cultureCode,
				   isMobile: query.PlatformType == PlatformType.Mobile,
				   gameCode: gameInfo.GameExternalId,
				   cancellationToken: cancellationToken)
			   .ConfigureAwait(false);
		}

		return !reply.IsPlayerExist
			? throw new Exception($"Player does not exist.{playerInfo.PlayerCode}")
			: reply.GameUrl;
	}
}
