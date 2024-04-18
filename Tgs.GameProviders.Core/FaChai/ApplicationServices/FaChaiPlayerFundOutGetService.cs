using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.FaChai.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.FaChai.ApplicationServices;

public class FaChaiPlayerFundOutGetService(
	IGameProviderRepository gameProvider,
	IPlayerRepository player,
	IBrandRepository brand,
	ISnowflakeWork snowflakeWork,
	IGameInterfaceFaChaiClient client) : IGetService<FaChaiPlayerFundOutQuery, PlayerFundResult>
{
	public async ValueTask<PlayerFundResult> GetAsync(FaChaiPlayerFundOutQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.FaChai,
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
		var configuration = await gameProvider.GetFaChaiConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var orderId = snowflakeWork.GetId();

		var reply = await client.FundOutAsync(
			apiUrl: configuration.ApiUrl,
			agentCode: configuration.AgentCode,
			agentKey: configuration.AgentKey,
			currency: configuration.Currency,
			playerCode: playerInfo.PlayerCode,
			orderId: orderId.ToString(),
			amount: query.Amount,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new PlayerFundResult()
		{
			OrderId = reply.TrsId
		};
	}
}
