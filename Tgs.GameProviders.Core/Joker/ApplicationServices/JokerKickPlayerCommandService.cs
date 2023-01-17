using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Joker.ApplicationServices;

internal class JokerKickPlayerCommandService : ICommandService<JokerKickPlayerCommand>
{
    private readonly IJokerAgentMapper _agentMapper;
    private readonly IJokerApiClient _client;
    private readonly IPlayerRepository _playerService;
    private readonly IJokerService _service;

    public JokerKickPlayerCommandService(
        IJokerApiClient client,
        IJokerService service,
        IJokerAgentMapper agentMapper,
        IPlayerRepository playerService)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _agentMapper = agentMapper ?? throw new ArgumentNullException(nameof(agentMapper));
        _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
    }

    public async ValueTask ExecuteAsync(
        JokerKickPlayerCommand command,
        CancellationToken cancellationToken = default)
    {
        var player = await _playerService
            .GetAsync(
                playerCode: command.PlayerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

        var agent = _agentMapper.GetAgentByCurrencyCode(
            configuration.BrandMerchants,
            player.BrandCode,
            player.DefaultWalletCurrencyCode);

        if (agent == null)
        {
            throw new Exception($"The agent does not exist of brand '{player.BrandCode}' and currency '{player.DefaultWalletCurrencyCode}'");
        }

        await _client.KickPlayerAsync(configuration.ApiUrl, player.PlayerCode, agent.AgentId, agent.SecretKey)
            .ConfigureAwait(false);
    }
}
