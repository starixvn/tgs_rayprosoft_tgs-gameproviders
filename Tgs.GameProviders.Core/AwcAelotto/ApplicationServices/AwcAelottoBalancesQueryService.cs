using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcAelotto.ApplicationServices;

internal class AwcAelottoBalancesQueryService(
	IBrandRepository brandRepository,
	IGameInterfaceAwcClient client,
	IGameProviderRepository gameProviderRepository) : IQueryService<AwcAelottoBalancesQuery, BalanceResult>
{
	public async IAsyncEnumerable<BalanceResult> QueryAsync(AwcAelottoBalancesQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.AWC_AELOTTO,
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

		var configuration = await gameProviderRepository.GetAwcAelottoConfigurationAsync(
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
