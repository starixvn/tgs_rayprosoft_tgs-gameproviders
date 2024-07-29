using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.CRSports.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.CRSports.ApplicationServices;

public class CRSportsPlayerOnlineGetService(
	IGameProviderRepository gameProvider,
	IPlayerRepository player,
	IBrandRepository brand,
	IGameInterfaceCRSportsClient client) : IGetService<CRSportsPlayerOnlineQuery, bool>
{
	public async ValueTask<bool> GetAsync(CRSportsPlayerOnlineQuery query, CancellationToken cancellationToken = default)
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

		return await client.PlayerOnlineAsync(
			apiUrl: configuration.ApiUrl,
			agentId: configuration.AgentId,
			secretKey: configuration.SecretKey,
			agentAccount: configuration.AgentAccount,
			agentPassword: configuration.AgentPassword,
			playerCode: playerInfo.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
