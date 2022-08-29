using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.TCG.ApplicationServices;

public class TCGKickPlayerCommandService : ICommandService<TCGKickPlayerCommand>
{
    private readonly ITCGApiClient _client;
    private readonly ILogger<TCGKickPlayerCommandService> _logger;
    private readonly IPlayerRepository _playerService;
    private readonly ITCGService _service;

    public TCGKickPlayerCommandService(
        ILogger<TCGKickPlayerCommandService> logger,
        ITCGApiClient client,
        ITCGService service,
        IPlayerRepository playerService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
    }

    public async ValueTask ExecuteAsync(TCGKickPlayerCommand command, CancellationToken cancellationToken = default)
    {
        var playerModel = await _playerService.GetAsync(command.PlayerCode).ConfigureAwait(false);
        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        var brandMerchant = await _service.GetMerchantAsync(playerModel.DefaultWalletCurrencyCode).ConfigureAwait(false);

        await _client.KickOutLottoPlayerAsync(
            configuration.ApiUrl,
            brandMerchant.MerchantCode,
            brandMerchant.DESKey,
            brandMerchant.SHA256Key,
            new()
            {
                UserName = command.PlayerCode
            }).ConfigureAwait(false);
    }
}
