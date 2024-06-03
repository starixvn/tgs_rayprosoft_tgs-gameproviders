using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Cards365.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Cards365Transfer.ApplicationServices;

public class Cards365TransferPlayerFundInGetService(
	IGameProviderRepository gameProvider,
	IPlayerRepository player,
	IBrandRepository brand,
	IGameInterfaceCards365Client client) : IGetService<Cards365TransferPlayerFundInQuery, PlayerFundResult>
{
	public async ValueTask<PlayerFundResult> GetAsync(Cards365TransferPlayerFundInQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.Cards365Transfer,
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
		var configuration = await gameProvider.GetCards365TransferConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await client.FundInAsync(
			apiUrl: configuration.ApiUrl,
			website: configuration.Website,
			cert: configuration.Cert,
			extension1: configuration.Extension1,
			playerCode: playerInfo.PlayerCode,
			transactionId: query.OrderId,
			amount: query.Amount,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new PlayerFundResult(
			OrderId: query.OrderId,
			ExternalOrderId: reply.TransactionId);
	}
}
