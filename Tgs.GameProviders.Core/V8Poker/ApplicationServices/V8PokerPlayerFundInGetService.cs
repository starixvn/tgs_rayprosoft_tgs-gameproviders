using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.V8Poker.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.V8Poker.ApplicationServices;

public class V8PokerPlayerFundInGetService(
	IGameProviderRepository gameProviderRepository,
	IPlayerRepository playerRepository,
	IBrandRepository brandRepository,
	IGameInterfaceV8PokerClient client) : IGetService<V8PokerPlayerFundInQuery, PlayerFundResult>
{
	public async ValueTask<PlayerFundResult> GetAsync(V8PokerPlayerFundInQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.V8Poker,
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
		var configuration = await gameProviderRepository.GetV8PokerConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		_ = await client
			.FundInAsync(
			apiUrl: configuration.ApiUrl,
			deskey: configuration.Deskey,
			md5key: configuration.Md5key,
			agentId: configuration.AgentId,
			playerCode: query.PlayerCode,
			orderId: query.OrderId,
			amount: query.Amount,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new PlayerFundResult(
			OrderId: query.OrderId,
			ExternalOrderId: string.Empty);
	}
}
