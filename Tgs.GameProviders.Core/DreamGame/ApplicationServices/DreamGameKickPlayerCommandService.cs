using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.DreamGamingService.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.DreamGame.ApplicationServices;

public class DreamGameKickPlayerCommandService : ICommandService<DreamGameKickPlayerCommand>
{
    private readonly IGameInterfaceDreamGamingClient _client;
    private readonly IDreamGameService _service;

    public DreamGameKickPlayerCommandService(
        IGameInterfaceDreamGamingClient client,
        IDreamGameService service)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(DreamGameKickPlayerCommand command, CancellationToken cancellationToken = default)
    {
        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

        var playerOnlines = await _client.PlayerOnlineAsync(
            apiUrl: configuration.ApiUrl,
            apiKey: configuration.ApiKey,
            agentName: configuration.AgentName,
            cancellationToken: cancellationToken)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        var playerOnline = playerOnlines.FirstOrDefault(p => p.PlayerCode == command.PlayerCode);

        if (playerOnline == null)
            return;

        await _client.PlayerKickAsync(
            apiUrl: configuration.ApiUrl,
            apiKey: configuration.ApiKey,
            agentName: configuration.AgentName,
            memberId: playerOnline.MemberId,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
