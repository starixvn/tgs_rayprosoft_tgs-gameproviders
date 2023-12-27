using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcRcb.ApplicationServices;

public class AwcRcbPlayerFundInGetService : IGetService<AwcRcbPlayerFundInQuery, PlayerFundResult>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceAwcClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public AwcRcbPlayerFundInGetService(
        IBrandRepository brand,
        IGameProviderRepository gameProvider,
        IGameInterfaceAwcClient client,
        IPlayerRepository player)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public async ValueTask<PlayerFundResult> GetAsync(AwcRcbPlayerFundInQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.AWC_RCB,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var player = await _player.GetAsync(
            playerCode: query.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(
            gameProviderId: gameProviderId,
            brandId: player.BrandId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var configuration = await _gameProvider.GetAwcRcbConfigurationAsync(
            gameProviderConfigurationId: gameProviderConfigurationId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var reply = await _client.FundInAsync(
            apiUrl: configuration.ApiUrl,
            agentId: configuration.AgentId,
            cert: configuration.Cert,
            playerCode: player.PlayerCode,
            orderId: query.OrderId,
            amount: query.Amount,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new PlayerFundResult
        {
            OrderId = reply.GameProviderTransactionId
        };
    }
}
