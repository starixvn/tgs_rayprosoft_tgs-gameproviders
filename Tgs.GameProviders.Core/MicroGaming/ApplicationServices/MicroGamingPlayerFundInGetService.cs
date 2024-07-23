using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.MicroGaming.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.MicroGaming.ApplicationServices;

public class MicroGamingPlayerFundInGetService(
	IGameProviderRepository gameProvider,
	IPlayerRepository player,
	IBrandRepository brand,
	IGameInterfaceMicroGamingClient client) : IGetService<MicroGamingPlayerFundInQuery, PlayerFundResult>
{
	public async ValueTask<PlayerFundResult> GetAsync(MicroGamingPlayerFundInQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.MicroGaming,
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
		var configuration = await gameProvider.GetMicroGamingConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await client.FundInAsync(
			stsApiBaseUrl: configuration.StsApiUrl,
			clientId: configuration.ClientId,
			clientSecret: configuration.ClientSecret,
			apiBaseUrl: configuration.ApiUrl,
			agentCode: configuration.AgentCode,
			playerCode: playerInfo.PlayerCode,
			amount: query.Amount,
			orderId: query.OrderId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new PlayerFundResult(
			OrderId: query.OrderId,
			ExternalOrderId: reply.ExternalOrderId);
	}
}
