using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Jili.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.JiliTransfer.ApplicationServices;

public class JiliTransferPlayerFundInGetService(
	IBrandRepository brandRepository,
	IGameProviderRepository gameProviderRepository,
	IGameInterfaceJiliClient client,
	IPlayerRepository playerRepository) : IGetService<JiliTransferPlayerFundInQuery, PlayerFundResult>
{
	public async ValueTask<PlayerFundResult> GetAsync(JiliTransferPlayerFundInQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.JiliTransfer,
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
		var configuration = await gameProviderRepository.GetJiliTransferConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await client.FundInAsync(
			apiUrl: configuration.ApiUrl,
			agentId: configuration.AgentId,
			agentKey: configuration.AgentKey,
			playerCode: playerInfo.PlayerCode,
			transactionId: query.OrderId,
			amount: query.Amount,
			cancellationToken: cancellationToken).ConfigureAwait(false);

		return new PlayerFundResult(
			OrderId: query.OrderId,
			ExternalOrderId: reply.TransactionId);
	}
}
