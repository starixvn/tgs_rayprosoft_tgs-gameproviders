using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.SkywindService.Client;
using Tgs.GameInterface.SkywindService.Protos;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SkywindTransfer.ApplicationServices;

public class SkywindTransferPlayerFundOutGetService(
	IGameProviderRepository gameProviderRepository,
	IPlayerRepository playerRepository,
	ISettingRepository settingRepository,
	IGameInterfaceSkywindClient client) : IGetService<SkywindTransferPlayerFundOutQuery, PlayerFundResult>
{
	public async ValueTask<PlayerFundResult> GetAsync(SkywindTransferPlayerFundOutQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.SkywindTransfer,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var playerInfo = await playerRepository.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await settingRepository.GetSkywindTransferConfigurationAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameProviderCurrencyCode = await gameProviderRepository
			.GetGameProviderCurrencyCodeAsync(
			gameProviderId: gameProviderId,
			currencyCode: playerInfo.DefaultWalletCurrencyCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(playerInfo.DefaultWalletCurrencyCode, $"This game product currency does not exist.");

		var request = new FundRequest()
		{
			ApiUrl = configuration.ApiUrl.ToString(),
			SecretKey = configuration.SecretKey,
			Username = configuration.Username,
			Password = configuration.Password,
			ExtTrxId = query.OrderId,
			PlayerCode = playerInfo.PlayerCode,
			Currency = gameProviderCurrencyCode,
			Amount = query.Amount,
		};

		var reply = await client.FundOutAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new PlayerFundResult(
			OrderId: query.OrderId,
			ExternalOrderId: reply.TrxId);
	}
}
