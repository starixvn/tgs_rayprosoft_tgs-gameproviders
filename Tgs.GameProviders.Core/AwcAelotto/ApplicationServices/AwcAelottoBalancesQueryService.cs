using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcAelotto.ApplicationServices;

internal class AwcAelottoBalancesQueryService : IQueryService<AwcAelottoBalancesQuery, BalanceResult>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceAwcServiceClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly ISettingRepository _setting;

    public AwcAelottoBalancesQueryService(
        IBrandRepository brand,
        IGameInterfaceAwcServiceClient client,
        IGameProviderRepository gameProvider,
        ISettingRepository setting)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _setting = setting ?? throw new ArgumentNullException(nameof(setting));
    }

    public async IAsyncEnumerable<BalanceResult> QueryAsync(
        AwcAelottoBalancesQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider
            .GetGameProviderIdAsync(
                gameProviderCode: GameProviderType.AWC_AELOTTO,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var brandId = await _brand
            .GetBrandIdAsync(
                brandCode: query.BrandCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand
            .GetBrandGameProviderConfigurationIdAsync(
                gameProviderId: gameProviderId,
                brandId: brandId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var configuration = await _setting
            .GetAwcAelottoConfigurationAsync(
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var configurationData = await _gameProvider
            .GetAwcConfigurationData(
                gameProviderConfigurationId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var reply = await _client
            .GetBalancesAsync(
                apiUrl: configuration.ApiUrl,
                agentId: configurationData.AgentId,
                cert: configurationData.Cert,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!reply.Items.Any())
        {
            yield break;
        }

        foreach (var item in reply.Items)
        {
            yield return new BalanceResult(
                PlayerCode: item.UserId,
                Balance: item.Balance.ToDecimal().GetValueOrDefault(0),
                LastDateTime:item.LastModified.ToDateTime());
        }
    }
}
