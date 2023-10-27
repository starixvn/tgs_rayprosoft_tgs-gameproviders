using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcKm.ApplicationServices;

internal class AwcKmBalancesQueryService : IQueryService<AwcKmBalancesQuery, BalanceResult>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceAwcServiceClient _client;
    private readonly IGameProviderRepository _gameProvider;

    public AwcKmBalancesQueryService(
        IBrandRepository brand,
        IGameInterfaceAwcServiceClient client,
        IGameProviderRepository gameProvider)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
    }

    public async IAsyncEnumerable<BalanceResult> QueryAsync(
        AwcKmBalancesQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.AWC_KM,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var brandId = await _brand.GetBrandIdAsync(
            brandCode: query.BrandCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(
            gameProviderId: gameProviderId,
            brandId: brandId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var configuration = await _gameProvider.GetAwcKmConfigurationAsync(
            gameProviderConfigurationId: gameProviderConfigurationId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var balanceInfos = await _client.AllBalancesAsync(
            apiUrl: configuration.ApiUrl,
            agentId: configuration.AgentId,
            cert: configuration.Cert,
            cancellationToken: cancellationToken)
            .ToArrayAsync(
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

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
