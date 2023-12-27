using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameInterface.Awc.Protos;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcBg.ApplicationServices;

internal class AwcBgBalancesQueryService(
    IBrandRepository brand,
    IGameInterfaceAwcClient client,
    IGameProviderRepository gameProvider) : IQueryService<AwcBgBalancesQuery, BalanceResult>
{
    public async IAsyncEnumerable<BalanceResult> QueryAsync(
        AwcBgBalancesQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var gameProviderId = await gameProvider.GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.AWC_BG,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var brandId = await brand.GetBrandIdAsync(
            brandCode: query.BrandCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderConfigurationId = await brand.GetBrandGameProviderConfigurationIdAsync(
            gameProviderId: gameProviderId,
            brandId: brandId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var configuration = await gameProvider.GetAwcBgConfigurationAsync(
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
            .ConfigureAwait(false)
            ?? Enumerable.Empty<BalanceInfo>();

        if (!balanceInfos.Any())
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
