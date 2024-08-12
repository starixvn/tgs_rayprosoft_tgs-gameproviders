using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.SabaService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SabaTransfer.ApplicationServices;

public class SabaTransferGameUrlGetService(
	IGameInterfaceSabaClient client,
	IGameProviderRepository gameProviderRepository,
	IPlayerRepository playerRepository,
	IBrandRepository brandRepository) : IGetService<SabaTransferGameUrlQuery, string>
{
	public async ValueTask<string> GetAsync(SabaTransferGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.SabaTransfer,
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
		var configuration = await gameProviderRepository.GetSabaTransferConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await gameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: gameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en";

		var reply = await client.GameUrlAsync(
			apiUrl: configuration.ApiUrl,
			operatorId: configuration.OperatorId,
			vendorId: configuration.VendorId,
			playerCode: player.PlayerCode,
			isMobile: query.PlatformType == PlatformType.Mobile,
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
				operatorId: configuration.OperatorId,
				vendorId: configuration.VendorId,
				playerCode: player.PlayerCode,
				isMobile: query.PlatformType == PlatformType.Mobile,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}

		return $"{reply.GameUrl}&lang={cultureCode}";
	}
}
