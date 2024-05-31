using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcPgs.ApplicationServices;

public class AwcPgsGameUrlGetService(
	IBrandRepository brand,
	IGameRepository game,
	IGameProviderRepository gameProvider,
	IGameInterfaceAwcClient client,
	IPlayerRepository player) : IGetService<AwcPgsGameUrlQuery, Uri>
{
	public async ValueTask<Uri> GetAsync(AwcPgsGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.AWC_PGS,
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

		var configuration = await gameProvider.GetAwcPgsConfigurationAsync(
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

		var reply = await client
			.GameUrlByAwcPgsAsync(
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

			reply = await client
			   .GameUrlByAwcPgsAsync(
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
			: new Uri(reply.GameUrl);
	}
}
