using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.SabaService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SabaSW.ApplicationServices;

public class SabaSWGameUrlGetService(
	IGameRepository gameRepository,
	IGameInterfaceSabaClient client,
	IGameProviderRepository gameProviderRepository,
	IPlayerRepository playerRepository,
	IBrandRepository brandRepository) : IGetService<SabaSWGameUrlQuery, string>
{
	public async ValueTask<string> GetAsync(SabaSWGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.SabaSW,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var player = await playerRepository.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameProviderConfigurationId = await brandRepository.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: gameProviderId,
			brandId: player.BrandId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await gameProviderRepository.GetSabaSWConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await gameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: gameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en";

		var gameInfo = await gameRepository.GetAsync(
			gameId: query.GameId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await client.GameUrlAsync(
			apiUrl: configuration.ApiUrl,
			vendorId: configuration.VendorId,
			playerCode: player.PlayerCode,
			isMobile: query.PlatformType == PlatformType.Mobile,
			brandCode: player.BrandCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!reply.IsPlayerExist)
		{
			var gameProviderCurrencyCode = await gameProviderRepository.GetGameProviderCurrencyCodeAsync(
				gameProviderId: gameProviderId,
				currencyCode: player.DefaultWalletCurrencyCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			await client.PlayerCreateAsync(
				configuration.ApiUrl,
				operatorId: configuration.OperatorId,
				vendorId: configuration.VendorId,
				brandCode: player.BrandCode,
				playerCode: player.PlayerCode,
				oddsType: configuration.OddsType,
				currency: int.Parse(gameProviderCurrencyCode),
				cancellationToken)
			   .ConfigureAwait(false);

			reply = await client.GameUrlAsync(
				apiUrl: configuration.ApiUrl,
				vendorId: configuration.VendorId,
				playerCode: player.PlayerCode,
				isMobile: query.PlatformType == PlatformType.Mobile,
				brandCode: player.BrandCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}

		var gameUrl = $"{reply.GameUrl}&lang={cultureCode}";

		return gameInfo.EndpointPath is null ? gameUrl : (gameUrl + gameInfo.EndpointPath);
	}
}
