using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.WMCasino.Models;
using Tgs.ServiceUtility;
using static Tgs.GameProviders.Core.WMCasino.Models.WMCasinoApiErrorCode;

namespace Tgs.GameProviders.Core.WMCasino.ApplicationServices;

public class WMCasinoGameUrlGetService : IGetService<WMCasinoGameUrlQuery, GameUrlResult>
{
    private readonly IWMCasinoAgentMapper _agentMapper;
    private readonly IWMCasinoApiClient _client;
    private readonly ILogger<WMCasinoGameUrlGetService> _logger;
    private readonly IWMCasinoGamePlayer _playerService;
    private readonly IWMCasinoService _service;
    private readonly IWMCasinoUtility _utility;

    public WMCasinoGameUrlGetService(
        ILogger<WMCasinoGameUrlGetService> logger,
        IWMCasinoService service,
        IWMCasinoAgentMapper agentMapper,
        IWMCasinoUtility utility,
        IWMCasinoGamePlayer playerService,
        IWMCasinoApiClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _agentMapper = agentMapper ?? throw new ArgumentNullException(nameof(agentMapper));
        _utility = utility ?? throw new ArgumentNullException(nameof(utility));
        _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<GameUrlResult> GetAsync(
        WMCasinoGameUrlQuery query, CancellationToken cancellationToken = default)
    {
        var response = await LoginGameAsync(query).ConfigureAwait(false);

        if (response is not { ErrorCode: (int)Success })
        {
            return null;
        }

        return new GameUrlResult
        {
            Url = new(response.Url)
        };
    }

    private async ValueTask<WMCasinoLoginGameResponse> LoginGameAsync(WMCasinoGameUrlQuery query)
    {
        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

        var player = await _service.GetPlayerAsync(query.PlayerCode).ConfigureAwait(false);

        var agent = _agentMapper.GetAgent(configuration.BrandMerchants, player.BrandCode,
            player.DefaultWalletCurrencyCode);

        var playerId = _utility.ToWMCasinoPlayerId(player.PlayerCode, agent.Prefix, agent.Suffix);

        var gameProviderCulture =
            await _service.GetGameProviderCultureCodeAsync(query.Language).ConfigureAwait(false);

        var request = new WMCasinoLoginGameRequest
        {
            VendorId = agent.AgentId,
            Signature = agent.Signature,
            Language = gameProviderCulture,
            User = playerId
        };

        var response = await _client.LoginGameAsync(configuration.ApiUrl, request).ConfigureAwait(false);

        if (response.ErrorCode is (int)Success or not (int)UserNotExist)
        {
            return response;
        }

        var created =
            await _playerService.CreatePlayerAsync(configuration.ApiUrl, agent, playerId, player.Username)
                .ConfigureAwait(false);

        return !created ? null : await _client.LoginGameAsync(configuration.ApiUrl, request).ConfigureAwait(false);
    }
}
