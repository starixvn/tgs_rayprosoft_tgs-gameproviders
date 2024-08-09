using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.PragmaticPlay.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;

internal class PragmaticPlayTransferGameUrlGetService(
	IGameProviderRepository gameProvider,
	IPlayerRepository player,
	IBrandRepository brand,
	IGameRepository game,
	IGameInterfacePragmaticPlayClient client) : IGetService<PragmaticPlayTransferGameUrlQuery, string>
{
	public async ValueTask<string> GetAsync(
		PragmaticPlayTransferGameUrlQuery query,
		CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.PragmaticPlayTransfer,
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
		var configuration = await gameProvider.GetPragmaticPlayTransferConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await gameProvider.GetGameProviderCultureCodeAsync(
			gameProviderId: gameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en";
		var gameInfo = await game.GetAsync(
			gameId: query.GameId!,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var response = await client.GameUrlAsync(
			apiUrl: configuration.ApiUrl,
			secureLogin: configuration.SecureLogin,
			secretKey: configuration.SecretKey,
			playerCode: query.PlayerCode,
			externalGameId: gameInfo.GameExternalId,
			language: cultureCode,
			isMobile: query.PlatformType == PlatformType.Mobile,
			lobbyUrl: query.BackUrl?.OriginalString,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!response.IsPlayerExist)
		{
			var currencyCode = await gameProvider.GetGameProviderCurrencyCodeAsync(
				gameProviderId: gameProviderId,
				currencyCode: playerInfo.DefaultWalletCurrencyCode,
				cancellationToken)
				.ConfigureAwait(false);

			await client.PlayerCreateAsync(
				apiUrl: configuration.ApiUrl,
				secureLogin: configuration.SecureLogin,
				secretKey: configuration.SecretKey,
				playerCode: query.PlayerCode,
				currencyCode: currencyCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			response = await client.GameUrlAsync(
				apiUrl: configuration.ApiUrl,
				secureLogin: configuration.SecureLogin,
				secretKey: configuration.SecretKey,
				playerCode: query.PlayerCode,
				externalGameId: gameInfo.GameExternalId,
				language: cultureCode,
				isMobile: query.PlatformType == PlatformType.Mobile,
				lobbyUrl: query.BackUrl?.OriginalString,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}

		return response.IsPlayerExist ? response.GameUrl : throw new Exception($"player does not exist.({playerInfo.PlayerCode})");
	}
}
