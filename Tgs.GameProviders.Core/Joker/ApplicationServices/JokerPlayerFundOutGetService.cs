using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Joker.ApplicationServices;

internal class JokerPlayerFundOutGetService : IGetService<JokerPlayerFundOutQuery, PlayerFundResult>
{
    private readonly IJokerApiClient _client;
    private readonly IJokerService _service;
    private readonly IJokerAgentMapper _agentMapper;

    public JokerPlayerFundOutGetService(
        IJokerApiClient client,
        IJokerService service,
        IJokerAgentMapper agentMapper)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _agentMapper = agentMapper ?? throw new ArgumentNullException(nameof(agentMapper));
    }

    public async ValueTask<PlayerFundResult> GetAsync(
        JokerPlayerFundOutQuery query,
        CancellationToken cancellationToken = default)
    {
        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

        var agent = _agentMapper.GetAgentByCurrencyCode(
            configuration.BrandMerchants,
            query.BrandCode,
            query.CurrencyCode);

        if (agent == null)
        {
            throw new Exception($"The agent does not exist of brand '{query.BrandCode}' and currency '{query.CurrencyCode}'");
        }

        var success = await _client.FundOutAsync(
            configuration.ApiUrl,
            query.PlayerCode,
            query.OrderId,
            query.Amount,
            agent.AgentId,
            agent.SecretKey).ConfigureAwait(false);

        return success
            ? new PlayerFundResult
            {
                OrderId = query.OrderId
            }
            : null;
    }
}