using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.AllBet.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AllBet.ApplicationServices;

internal class AllBetGameUrlGetService : IGetService<AllBetGameUrlQuery, GameUrlResult>
{
    private readonly IAllBetAgentMapper _agentMapper;
    private readonly IAllBetApiClient _client;
    private readonly ILogger<AllBetGameUrlGetService> _logger;
    private readonly IAllBetGamePlayer _playerService;
    private readonly IAllBetService _service;

    public AllBetGameUrlGetService(
        ILogger<AllBetGameUrlGetService> logger,
        IAllBetService service,
        IAllBetAgentMapper agentMapper,
        IAllBetGamePlayer playerService,
        IAllBetApiClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _agentMapper = agentMapper ?? throw new ArgumentNullException(nameof(agentMapper));
        _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<GameUrlResult> GetAsync(AllBetGameUrlQuery query, CancellationToken cancellationToken = default)
    {
        var response = await LoginGameAsync(query).ConfigureAwait(false);

        if (response is not { ErrorCode: nameof(AllBetApiErrorCode.OK) })
        {
            return null;
        }

        return new GameUrlResult
        {
            Url = new Uri(response.Data.Url)
        };
    }

    private async ValueTask<AllBetLoginResponse> LoginGameAsync(AllBetGameUrlQuery query)
    {
        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

        var agent = _agentMapper.GetAgent(configuration.BrandMerchants, query.BrandCode, query.CurrencyCode);

        var playerId = _service.ToAllBetPlayerId(query.PlayerCode, agent.Suffix);
        var cultureCode = await _service.GetGameProviderLanguageAsync(query.Language).ConfigureAwait(false);

        var request = new AllBetLoginRequest
        {
            PlayerId = playerId,
            Language = cultureCode
        };

        var response = await _client
            .LoginAsync(configuration.ApiUrl, agent.PropertyId, agent.AllBetApiKey, request)
            .ConfigureAwait(false);

        if (response.ErrorCode is nameof(AllBetApiErrorCode.OK) or not nameof(AllBetApiErrorCode.PLAYER_NOT_EXIST))
        {
            return response;
        }

        var created = await _playerService
            .CheckOrCreateMemberAsync(playerId, configuration.ApiUrl, agent)
            .ConfigureAwait(false);

        return !created ?
            null :
            await _client
                .LoginAsync(configuration.ApiUrl, agent.PropertyId, agent.AllBetApiKey, request)
                .ConfigureAwait(false);
    }
}
