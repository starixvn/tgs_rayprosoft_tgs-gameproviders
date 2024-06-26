using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.SabaService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SabaTransfer.ApplicationServices;

public class SabaTransferPlayerFundOutGetService(
	IGameInterfaceSabaClient client,
	IGameProviderRepository gameProviderRepository,
	IPlayerRepository playerRepository,
	IBrandRepository brandRepository) : IGetService<SabaTransferPlayerFundOutQuery, PlayerFundResult>
{
	public async ValueTask<PlayerFundResult> GetAsync(SabaTransferPlayerFundOutQuery query, CancellationToken cancellationToken = default)
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
		var gameProviderCurrencyCode = await gameProviderRepository.GetGameProviderCurrencyCodeAsync(
			gameProviderId: gameProviderId,
			currencyCode: player.DefaultWalletCurrencyCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await client.FundOutAsync(
			apiUrl: configuration.ApiUrl,
			operatorId: configuration.OperatorId,
			vendorId: configuration.VendorId,
			playerCode: player.PlayerCode,
			orderId: query.OrderId,
			currency: int.Parse(gameProviderCurrencyCode),
			amount: query.Amount,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!reply.IsOrderExist)
		{
			if (reply.Check)
			{
				throw new Exception($"請呼叫遊戲廠商提供CheckFundTransfer.({JsonSerializer.Serialize(reply)})");
			}

			throw new Exception($"遊戲廠商交易失敗.({JsonSerializer.Serialize(reply)})");
		}

		return new PlayerFundResult(
			OrderId: query.OrderId,
			ExternalOrderId: reply.VendorTransId);
	}
}
