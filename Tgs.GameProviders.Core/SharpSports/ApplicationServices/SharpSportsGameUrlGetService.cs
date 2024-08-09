using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.SharpSports.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SharpSports.ApplicationServices;

public class SharpSportsGameUrlGetService(
	IGameProviderRepository gameProviderRepository,
	IPlayerRepository playerRepository,
	IBrandRepository brandRepository,
	IGameInterfaceSharpSportsClient client) : IGetService<SharpSportsGameUrlQuery, string>
{
	public async ValueTask<string> GetAsync(SharpSportsGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.SharpSports,
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
		var configuration = await gameProviderRepository.GetSharpSportsConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await gameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: gameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en-us";

		var reply = await client.GameUrlAsync(
			apiUrl: configuration.ApiUrl,
			merchantId: configuration.MerchantId,
			clientId: configuration.ClientId,
			clientSecret: configuration.ClientSecret,
			playerCode: playerInfo.PlayerCode,
			isMobile: query.PlatformType == PlatformType.Mobile,
			language: cultureCode,
			ip: query.IPAddress,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return reply.GameUrl;
	}
}
