using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Avia.ApplicationServices;

public class AviaKickPlayerCommandService : ICommandService<AviaKickPlayerCommand>
{
    private readonly IAviaApiClient _client;
    private readonly ILogger<AviaKickPlayerCommandService> _logger;
    private readonly IPlayerRepository _playerService;
    private readonly IAviaService _service;

    public AviaKickPlayerCommandService(
        ILogger<AviaKickPlayerCommandService> logger,
        IAviaApiClient client,
        IAviaService service,
        IPlayerRepository playerService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
    }

    public async ValueTask ExecuteAsync(AviaKickPlayerCommand command, CancellationToken cancellationToken = default)
    {
        var playerModel = await _playerService.GetAsync(command.PlayerCode).ConfigureAwait(false);
        var brandMerchant = await _service.GetBrandMerchantAsync(playerModel.BrandCode);

        if (brandMerchant == null)
        {
            throw new ArgumentNullException(nameof(brandMerchant));
        }

        await _client.KickAsync(
            brandMerchant.ApiUrl,
            brandMerchant.Authorization,
            command.PlayerCode).ConfigureAwait(false);
    }
}
