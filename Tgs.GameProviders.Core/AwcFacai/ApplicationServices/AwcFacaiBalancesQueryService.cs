using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcFacai.ApplicationServices;

internal class AwcFacaiBalancesQueryService(
	IBrandRepository brandRepository,
	IGameInterfaceAwcClient client,
	IGameProviderRepository gameProviderRepository) : IQueryService<AwcFacaiBalancesQuery, BalanceResult>
{
	public async IAsyncEnumerable<BalanceResult> QueryAsync(
		AwcFacaiBalancesQuery query,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.AWC_FACAI,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var brandId = await brandRepository.GetBrandIdAsync(
			brandCode: query.BrandCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameProviderConfigurationId = await brandRepository.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: gameProviderId,
			brandId: brandId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var configuration = await gameProviderRepository.GetAwcFacaiConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var balanceInfos = await client.AllBalancesAsync(
			apiUrl: configuration.ApiUrl,
			agentId: configuration.AgentId,
			cert: configuration.Cert,
			cancellationToken: cancellationToken)
			.ToArrayAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (balanceInfos.Length == 0)
		{
			yield break;
		}

		foreach (var balanceInfo in balanceInfos)
		{
			yield return new BalanceResult(
				PlayerCode: balanceInfo.PlayerCode,
				Balance: balanceInfo.Balance.ToDecimal().GetValueOrDefault(0),
				LastDateTime: balanceInfo.LastModified.ToDateTime());
		}
	}
}
