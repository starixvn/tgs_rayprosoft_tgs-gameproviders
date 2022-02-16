using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Habanero.Models;

namespace TGS.GameProviders.Core.Habanero.ApplicationServices;

internal class HabaneroKickPlayerCommandService : ICommandService<HabaneroKickPlayerCommand>
{
    private readonly IHabaneroApiClient _client;
    private readonly ILogger<HabaneroKickPlayerCommandService> _logger;
    private readonly IHabaneroService _service;

    public HabaneroKickPlayerCommandService(
        ILogger<HabaneroKickPlayerCommandService> logger,
        IHabaneroService service,
        IHabaneroApiClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(
        HabaneroKickPlayerCommand command,
        CancellationToken cancellationToken = default)
    {
        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

        var request = new HabaneroLogoutRequest
        {
            BrandId = configuration.BrandId,
            ApiKey = configuration.ApiKey,
            PlayerCode = command.PlayerCode
        };

        _ = await _client.LogoutAsync(configuration.ApiUrl, request).ConfigureAwait(false);
    }
}
