using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Saba.Models;

namespace TGS.GameProviders.Core.Saba.ApplicationServices;

internal class SabaKickPlayerCommandService : ICommandService<SabaKickPlayerCommand>
{
    private readonly ISabaApiClient _client;
    private readonly ILogger<SabaKickPlayerCommandService> _logger;
    private readonly IPlayerService _playerService;
    private readonly ISabaService _service;
    private readonly ISabaUtility _utility;

    public SabaKickPlayerCommandService(
        ILogger<SabaKickPlayerCommandService> logger,
        IPlayerService playerService,
        ISabaService service,
        ISabaUtility utility,
        ISabaApiClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _utility = utility ?? throw new ArgumentNullException(nameof(utility));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(SabaKickPlayerCommand command, CancellationToken cancellationToken = default)
    {
        var playerModel = await _playerService.GetCacheAsync(command.PlayerCode).ConfigureAwait(false);
        if (playerModel == null)
        {
            throw new Exception($"The player does not exist.{command.PlayerCode}");
        }

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

        var playerId = _utility.ToSabaPlayerId(command.PlayerCode, configuration.IsTestEnvironment);

        var request = new SabaKickUserRequest
        {
            VendorId = configuration.VendorId,
            PlayerId = playerId
        };

        _ = await _client.KickUserAsync(configuration.ApiUrl, request).ConfigureAwait(false);
    }
}
