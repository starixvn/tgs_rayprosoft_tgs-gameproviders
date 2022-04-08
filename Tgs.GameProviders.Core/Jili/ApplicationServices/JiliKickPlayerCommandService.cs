using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Jili.ApplicationServices;

internal class JiliKickPlayerCommandService : ICommandService<JiliKickPlayerCommand>
{
    private readonly IJiliApiClient _client;
    private readonly ILogger<JiliKickPlayerCommandService> _logger;
    private readonly IJiliMerchantMapper _merchantMapper;
    private readonly IPlayerService _playerService;
    private readonly IJiliService _service;

    public JiliKickPlayerCommandService(
        ILogger<JiliKickPlayerCommandService> logger,
        IPlayerService playerService,
        IJiliService service,
        IJiliMerchantMapper merchantMapper,
        IJiliApiClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _merchantMapper = merchantMapper ?? throw new ArgumentNullException(nameof(merchantMapper));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(
        JiliKickPlayerCommand command,
        CancellationToken cancellationToken = default)
    {
        var playerInfo = await _playerService.GetCacheAsync(command.PlayerCode).ConfigureAwait(false);
        if (playerInfo == null)
        {
            throw new Exception($"The player does not exist.{command.PlayerCode}");
        }

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        var merchant = _merchantMapper.GetMerchant(configuration.Merchants, playerInfo.BrandCode);
        var jiliPlayerId = $"{playerInfo.BrandCode}_{playerInfo.PlayerCode}";

        await _client.KickMemberAsync(
                configuration.ApiUrl,
                jiliPlayerId,
                merchant.AgentId,
                merchant.AgentKey)
            .ConfigureAwait(false);
    }
}
