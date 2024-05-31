using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.CQ9Gaming.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.CQ9Gaming.ApplicationServices;

public class CQ9GamingPlayerFundOutGetService(
	IGameProviderRepository gameProvider,
	IPlayerRepository player,
	IBrandRepository brand,
	IGameInterfaceCQ9GamingClient client) : IGetService<CQ9GamingPlayerFundOutQuery, PlayerFundResult>
{
	public async ValueTask<PlayerFundResult> GetAsync(CQ9GamingPlayerFundOutQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.CQ9Gaming,
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
		var configuration = await gameProvider.GetCQ9GamingConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		_ = await client.FundOutAsync(
		   apiUrl: configuration.ApiUrl,
		   apiToken: configuration.ApiToken,
		   playerCode: playerInfo.PlayerCode,
		   orderId: query.OrderId,
		   amount: query.Amount,
		   cancellationToken: cancellationToken)
		   .ConfigureAwait(false);

		return new PlayerFundResult(
			GameProviderTransactionId: query.OrderId,
			ExternalGameProviderTransactionId: string.Empty);
	}
}
