using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcKm.ApplicationServices;

public class AwcKmGameUrlGetService(
	IBrandRepository brand,
	IGameRepository game,
	IGameProviderRepository gameProvider,
	IGameInterfaceAwcClient client,
	IPlayerRepository player) : IGetService<AwcKmGameUrlQuery, string>
{
	public async ValueTask<string> GetAsync(AwcKmGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.AWC_KM,
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

		var configuration = await gameProvider.GetAwcKmConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var cultureCode = await gameProvider.GetGameProviderCultureCodeAsync(
			gameProviderId: gameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en";

		var gameInfo = await game.GetAsync(
			gameId: query.GameId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await client.GameUrlByAwcKmAsync(
			apiUrl: configuration.ApiUrl,
			agentId: configuration.AgentId,
			cert: configuration.Cert,
			playerCode: playerInfo.PlayerCode,
			isMobileLogin: query.PlatformType == PlatformType.Mobile,
			backUrl: query.BackUrl.ToString(),
			externalGameId: gameInfo.GameExternalId,
			language: cultureCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!reply.IsPlayerExist)
		{
			var currency = await gameProvider.GetGameProviderCurrencyCodeAsync(
				gameProviderId: gameProviderId,
				currencyCode: playerInfo.DefaultWalletCurrencyCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			await client.PlayerCreateAsync(
				apiUrl: configuration.ApiUrl,
				agentId: configuration.AgentId,
				cert: configuration.Cert,
				playerCode: query.PlayerCode,
				userName: query.PlayerCode,
				currency: currency,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			reply = await client.GameUrlByAwcKmAsync(
			   apiUrl: configuration.ApiUrl,
			   agentId: configuration.AgentId,
			   cert: configuration.Cert,
			   playerCode: playerInfo.PlayerCode,
			   isMobileLogin: query.PlatformType == PlatformType.Mobile,
			   backUrl: query.BackUrl.ToString(),
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
