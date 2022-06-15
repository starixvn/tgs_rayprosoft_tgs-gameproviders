using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Joker.ApplicationServices;

internal class JokerPlayerBalanceGetService : IGetService<JokerPlayerBalanceQuery, decimal?>
{
    private readonly IJokerAgentMapper _agentMapper;
    private readonly IJokerApiClient _client;
    private readonly IJokerGamePlayer _gamePlayer;
    private readonly IPlayerRepository _playerService;
    private readonly IJokerService _service;

    public JokerPlayerBalanceGetService(
        IJokerApiClient client,
        IJokerService service,
        IJokerGamePlayer gamePlayer,
        IJokerAgentMapper agentMapper,
        IPlayerRepository playerService)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _gamePlayer = gamePlayer ?? throw new ArgumentNullException(nameof(gamePlayer));
        _agentMapper = agentMapper ?? throw new ArgumentNullException(nameof(agentMapper));
        _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
    }

    public async ValueTask<decimal?> GetAsync(
        JokerPlayerBalanceQuery query,
        CancellationToken cancellationToken = default)
    {
        var playerModel = await _playerService.GetAsync(query.PlayerCode).ConfigureAwait(false);
        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

        var agent = _agentMapper.GetAgentByCurrencyCode(
            configuration.BrandMerchants,
            playerModel.BrandCode,
            playerModel.DefaultWalletCurrencyCode);

        if (agent == null)
        {
            throw new Exception($"The agent does not exist of brand '{playerModel.BrandCode}' and currency '{playerModel.DefaultWalletCurrencyCode}'");
        }

        var (isPlayerExist, balance) = await _client
            .GetBalanceAsync(configuration.ApiUrl, playerModel.PlayerCode, agent.AgentId, agent.SecretKey)
            .ConfigureAwait(false);

        if (isPlayerExist is not false)
        {
            return balance;
        }

        var created = await _gamePlayer.CreatePlayerAsync(playerModel.PlayerCode, configuration.ApiUrl, agent)
            .ConfigureAwait(false);

        if (created)
        {
            (_, balance) = await _client
                .GetBalanceAsync(configuration.ApiUrl, playerModel.PlayerCode, agent.AgentId, agent.SecretKey)
                .ConfigureAwait(false);
        }

        return balance;
    }
}
