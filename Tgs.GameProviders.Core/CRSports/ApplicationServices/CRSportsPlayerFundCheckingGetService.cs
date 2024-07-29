using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.CRSports.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.CRSports.ApplicationServices;

public class CRSportsPlayerFundCheckingGetService(
	IGameProviderRepository gameProvider,
	IPlayerRepository player,
	IBrandRepository brand,
	IGameInterfaceCRSportsClient client) : IGetService<CRSportsPlayerFundCheckingQuery, bool>
{
	public async ValueTask<bool> GetAsync(CRSportsPlayerFundCheckingQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.CRSports,
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
		var configuration = await gameProvider.GetCRSportsConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await client
			.FundStatusAsync(
				apiUrl: configuration.ApiUrl,
				agentId: configuration.AgentId,
				secretKey: configuration.SecretKey,
				agentAccount: configuration.AgentAccount,
				agentPassword: configuration.AgentPassword,
				playerCode: playerInfo.PlayerCode,
				orderId: query.OrderId,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return reply;
	}
}
