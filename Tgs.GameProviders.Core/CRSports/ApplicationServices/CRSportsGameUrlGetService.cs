using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.CRSports.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.CRSports.ApplicationServices;

public class CRSportsGameUrlGetService(
	IGameProviderRepository gameProvider,
	IPlayerRepository player,
	IBrandRepository brand,
	IGameInterfaceCRSportsClient client) : IGetService<CRSportsGameUrlQuery, Uri>
{
	public async ValueTask<Uri> GetAsync(CRSportsGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.CRSports,
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
		var configuration = await gameProvider.GetCRSportsConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await gameProvider.GetGameProviderCultureCodeAsync(
			gameProviderId: gameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en";
		var currency = await gameProvider.GetGameProviderCurrencyCodeAsync(
			gameProviderId: gameProviderId,
			currencyCode: playerInfo.DefaultWalletCurrencyCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await client.GameUrlAsync(
			apiUrl: configuration.ApiUrl,
			agentId: configuration.AgentId,
			secretKey: configuration.SecretKey,
			agentAccount: configuration.AgentAccount,
			agentPassword: configuration.AgentPassword,
			playerCode: playerInfo.PlayerCode,
			currency: currency,
			language: cultureCode,
			isMobile: query.PlatformType == PlatformType.Mobile,
			ipAddress: query.IPAddress,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new Uri(reply.GameUrl);
	}
}
