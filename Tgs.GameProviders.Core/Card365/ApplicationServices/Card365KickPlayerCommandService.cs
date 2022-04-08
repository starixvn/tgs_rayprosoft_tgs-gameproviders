using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.Card365.Model;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Card365.ApplicationServices;

internal class Card365KickPlayerCommandService : ICommandService<Card365KickPlayerCommand>
{
    private readonly ICard365AgentMapper _agentMapper;
    private readonly ICard365ApiClient _client;
    private readonly ILogger<Card365KickPlayerCommandService> _logger;
    private readonly IPlayerService _playerService;
    private readonly ICard365Service _service;

    public Card365KickPlayerCommandService(
        ILogger<Card365KickPlayerCommandService> logger,
        IPlayerService playerService,
        ICard365Service service,
        ICard365AgentMapper agentMapper,
        ICard365ApiClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _agentMapper = agentMapper ?? throw new ArgumentNullException(nameof(agentMapper));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(
        Card365KickPlayerCommand command,
        CancellationToken cancellationToken = default)
    {
        var playerInfo = await _playerService.GetCacheAsync(command.PlayerCode).ConfigureAwait(false);
        if (playerInfo == null)
        {
            throw new Exception($"The player does not exist.{command.PlayerCode}");
        }

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

        var agent = _agentMapper.GetAgent(configuration.BrandAgents, playerInfo.BrandCode);

        var request = new Card365LogoutRequest
        {
            Cert = configuration.Cert,
            AgentId = agent.AgentId,
            PlayerCode = playerInfo.PlayerCode
        };

        _ = await _client.LogoutAsync(
                configuration.ApiUrl,
                configuration.Website,
                request)
            .ConfigureAwait(false);
    }
}
