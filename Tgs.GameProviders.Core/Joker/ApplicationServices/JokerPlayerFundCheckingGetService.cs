using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Joker.ApplicationServices;

internal class JokerPlayerFundCheckingGetService : IGetService<JokerPlayerFundCheckingQuery, PlayerFundCheckingResult>
{
    private readonly IJokerApiClient _client;
    private readonly IJokerService _service;
    private readonly IJokerAgentMapper _agentMapper;

    public JokerPlayerFundCheckingGetService(
        IJokerApiClient client,
        IJokerService service,
        IJokerAgentMapper agentMapper)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _agentMapper = agentMapper ?? throw new ArgumentNullException(nameof(agentMapper));
    }

    public async ValueTask<PlayerFundCheckingResult> GetAsync(
        JokerPlayerFundCheckingQuery query,
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

        var isExist = await _client
            .FundOrderCheckingAsync(
                configuration.ApiUrl,
                query.OrderId,
                agent.AgentId,
                agent.SecretKey)
            .ConfigureAwait(false);

        return new PlayerFundCheckingResult
        {
            OrderStatus = isExist
        };
    }
}