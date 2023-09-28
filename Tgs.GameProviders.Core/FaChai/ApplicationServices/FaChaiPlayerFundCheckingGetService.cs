using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.FaChai.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.FaChai.ApplicationServices;

public class FaChaiPlayerFundCheckingGetService : IGetService<FaChaiPlayerFundCheckingQuery, bool>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceFaChaiClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public FaChaiPlayerFundCheckingGetService(
        IGameProviderRepository gameProvider,
        IPlayerRepository player,
        IBrandRepository brand,
        IGameInterfaceFaChaiClient client)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public async ValueTask<bool> GetAsync(FaChaiPlayerFundCheckingQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider
            .GetGameProviderIdAsync(
                gameProviderCode: GameProviderType.FaChai,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var player = await _player
            .GetAsync(
                playerCode: query.PlayerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand
            .GetBrandGameProviderConfigurationIdAsync(
                gameProviderId: gameProviderId,
                brandId: player.BrandId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var configuration = await _gameProvider
            .GetFaChaiConfigurationAsync(
                gameProviderConfigurationId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await _client
            .FundStatusAsync(
            apiUrl: configuration.ApiUrl,
            agentCode: configuration.AgentCode,
            agentKey: configuration.AgentKey,
            currency: configuration.Currency,
            trsId: query.OrderId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
