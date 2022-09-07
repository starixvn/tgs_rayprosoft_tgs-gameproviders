using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SexyGaming.ApplicationServices;

public class SexyGamingKickPlayerCommandService : ICommandService<SexyGamingKickPlayerCommand>
{
    private readonly ISexyGamingApiClient _client;
    private readonly ILogger<SexyGamingKickPlayerCommandService> _logger;

    private readonly ISexyGamingService _service;

    public SexyGamingKickPlayerCommandService(
        ILogger<SexyGamingKickPlayerCommandService> logger,
        ISexyGamingApiClient client,
        ISexyGamingService service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(SexyGamingKickPlayerCommand command, CancellationToken cancellationToken = default)
    {
        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

        await _client.LogoutAsync(configuration.ApiUrl, new()
        {
            AgentId = configuration.AgentId,
            Cert = configuration.Cert,
            PlayerCode = command.PlayerCode
        }).ConfigureAwait(false);
    }
}
