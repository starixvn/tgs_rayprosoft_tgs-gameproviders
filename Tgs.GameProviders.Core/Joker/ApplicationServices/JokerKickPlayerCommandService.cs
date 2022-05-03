using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Joker.ApplicationServices;

internal class JokerKickPlayerCommandService : ICommandService<JokerKickPlayerCommand>
{
    private readonly IJokerApiClient _client;
    private readonly IJokerService _service;
    private readonly IJokerAgentMapper _agentMapper;
    private readonly IPlayerService _playerService;

    public JokerKickPlayerCommandService(
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

    public async ValueTask ExecuteAsync(
        JokerKickPlayerCommand command,
        CancellationToken cancellationToken = default)
    {
        var playerInfo = await _playerService.GetCacheAsync(command.PlayerCode).ConfigureAwait(false);

        if (playerInfo == null)
        {
            throw new Exception($"The player does not exist.{command.PlayerCode}");
        }

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

        var agent = _agentMapper.GetAgentByCurrencyCode(
            configuration.BrandMerchants,
            playerInfo.BrandCode,
            playerInfo.DefaultWalletCurrencyCode);

        if (agent == null)
        {
            throw new Exception($"The agent does not exist of brand '{playerInfo.BrandCode}' and currency '{playerInfo.DefaultWalletCurrencyCode}'");
        }

        await _client.KickPlayerAsync(configuration.ApiUrl, playerInfo.PlayerCode, agent.AgentId, agent.SecretKey)
            .ConfigureAwait(false);
    }
}