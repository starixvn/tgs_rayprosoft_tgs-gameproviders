using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.SBOService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SBOTransfer.ApplicationServices;

public class SBOTransferPlayerFundInGetService(
	IGameInterfaceSBOClient client,
	IGameProviderRepository gameProviderRepository,
	IPlayerRepository playerRepository,
	IBrandRepository brandRepository) : IGetService<SBOTransferPlayerFundInQuery, PlayerFundResult>
{
	public async ValueTask<PlayerFundResult> GetAsync(SBOTransferPlayerFundInQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.SBOTransfer,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var playerInfo = await playerRepository.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameProviderConfigurationId = await brandRepository.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: gameProviderId,
			playerInfo.BrandId,
			cancellationToken)
			.ConfigureAwait(false);
		var configuration = await gameProviderRepository.GetSBOTransferConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await client.FundInAsync(
			configuration.ApiUrl,
			configuration.CompanyKey,
			configuration.Agent,
			playerInfo.PlayerCode,
			orderId: query.OrderId,
			query.Amount,
			cancellationToken)
			.ConfigureAwait(false);

		return new PlayerFundResult(
			GameProviderTransactionId: query.OrderId,
			ExternalGameProviderTransactionId: reply.TxnId);
	}
}
