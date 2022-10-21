using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.BTiService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.BTiSports.ApplicationServices;

public class BTiSportsPlayerFundCheckingGetService : IGetService<BTiSportsPlayerFundCheckingQuery, PlayerFundCheckingResult>
{
    private readonly IBrandRepository _brand;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IGameInterfaceBTiClient _client;
    private readonly IPlayerRepository _player;

    public BTiSportsPlayerFundCheckingGetService(
        IBrandRepository brand,
        IGameProviderRepository gameProvider,
        IGameInterfaceBTiClient client,
        IPlayerRepository player)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public async ValueTask<PlayerFundCheckingResult> GetAsync(BTiSportsPlayerFundCheckingQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.BTiSports).ConfigureAwait(false);
        var player = await _player.GetAsync(query.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configuration = await _gameProvider.GetBTiSportsConfiguration(gameProviderConfigurationId).ConfigureAwait(false);

        var response = await _client
            .FundStatusAsync(
                apiUrl: configuration.ApiUrl,
                agentUserName: configuration.AgentUserName,
                agentPassword: configuration.AgentPassword,
                refTransactionCode: query.OrderId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new() { OrderStatus = response };
    }
}
