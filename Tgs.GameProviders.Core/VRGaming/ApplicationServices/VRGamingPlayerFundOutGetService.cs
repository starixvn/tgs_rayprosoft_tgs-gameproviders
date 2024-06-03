using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.VRGaming.Client;
using Tgs.GameProviders.Domains;

using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.VRGaming.ApplicationServices;

public class VRGamingPlayerFundOutGetService(
	IGameProviderRepository gameProvider,
	IPlayerRepository player,
	IBrandRepository brand,
	IGameInterfaceVRGamingClient client) : IGetService<VRGamingPlayerFundOutQuery, PlayerFundResult>
{
	public async ValueTask<PlayerFundResult> GetAsync(VRGamingPlayerFundOutQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.VRGaming,
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
		var configuration = await gameProvider.GetVRGamingConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await client.FundOutAsync(
			 configuration.ApiUrl,
			 configuration.Version,
			 configuration.MerchantId,
			 configuration.Deskey,
			 playerInfo.PlayerCode,
			 orderId: query.OrderId,
			 amount: query.Amount,
			 cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new PlayerFundResult(
			OrderId: query.OrderId,
			ExternalOrderId: string.Empty);
	}
}
