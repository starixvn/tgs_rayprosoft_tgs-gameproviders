using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Joker.ApplicationServices;

internal class JokerPlayerBalanceGetService : IGetService<JokerPlayerBalanceQuery, decimal?>
{
    private readonly IJokerApiClient _client;
    private readonly IJokerAgentMapper _agentMapper;
    private readonly IJokerService _service;
    private readonly IPlayerService _playerService;

    public JokerPlayerBalanceGetService(
        IJokerApiClient client,
        IJokerService service,
        IJokerAgentMapper agentMapper,
        IPlayerService playerService)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _agentMapper = agentMapper ?? throw new ArgumentNullException(nameof(agentMapper));
        _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
    }

    public async ValueTask<decimal?> GetAsync(
        JokerPlayerBalanceQuery query,
        CancellationToken cancellationToken = default)
    {
        var playerInfo = await _playerService.GetCacheAsync(query.PlayerCode).ConfigureAwait(false);

        if (playerInfo == null)
        {
            throw new Exception($"The player does not exist.{query.PlayerCode}");
        }

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

        var agent = _agentMapper.GetAgent(
            configuration.BrandMerchants,
            playerInfo.BrandCode,
            playerInfo.DefaultWalletCurrencyCode);

        if (agent == null)
        {
            throw new Exception($"The agent does not exist of brand '{playerInfo.BrandCode}' and currency '{playerInfo.DefaultWalletCurrencyCode}'");
        }

        return await _client
            .GetBalanceAsync(configuration.ApiUrl, playerInfo.PlayerCode, agent.AgentId, agent.SecretKey)
            .ConfigureAwait(false);
    }
}