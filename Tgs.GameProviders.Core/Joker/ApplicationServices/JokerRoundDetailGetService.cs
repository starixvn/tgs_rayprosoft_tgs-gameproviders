using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Joker.ApplicationServices;

internal class JokerRoundDetailGetService : IGetService<JokerRoundDetailQuery, RoundDetailResult>
{
    private readonly IJokerApiClient _client;
    private readonly IJokerService _service;
    private readonly IJokerAgentMapper _agentMapper;
    private readonly IPlayerService _playerService;
    private readonly IRoundService _roundService;

    public JokerRoundDetailGetService(
        IJokerApiClient client,
        IJokerService service,
        IJokerAgentMapper agentMapper,
        IPlayerService playerService,
        IRoundService roundService)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _agentMapper = agentMapper ?? throw new ArgumentNullException(nameof(agentMapper));
        _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
    }

    public async ValueTask<RoundDetailResult> GetAsync(
        JokerRoundDetailQuery query,
        CancellationToken cancellationToken = default)
    {
        var roundInfo = await _roundService.GetAsync(query.RoundId).ConfigureAwait(false);
        if (roundInfo == null)
        {
            throw new Exception($"This game round number could not be found.{query.RoundId}");
        }

        var playerInfo = await _playerService.GetAsync(roundInfo.PlayerCode).ConfigureAwait(false);
        if (playerInfo == null)
        {
            throw new Exception($"This player could not be found.{roundInfo.PlayerCode}");
        }

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

        var agent = _agentMapper.GetAgent(
            configuration!.BrandMerchants,
            playerInfo.BrandCode,
            roundInfo.CurrencyCode);

        if (agent == null)
        {
            throw new Exception($"The agent does not exist of brand '{playerInfo.BrandCode}' and currency '{roundInfo.CurrencyCode}'");
        }

        var gameProviderCultureCode = await _service
            .GetGameProviderLanguageCodeAsync(query.Language)
            .ConfigureAwait(false);

        var document = JsonDocument.Parse(roundInfo.AdditionalData);
        var type = document.RootElement.GetProperty("Type").GetString();
        if (!Enum.TryParse<JokerDataType>(type, true, out var dataType))
        {
            dataType = JokerDataType.Game;
        }

        var url = await _client
            .GetGameDetailUrlAsync(configuration.ApiUrl,
                gameProviderCultureCode,
                roundInfo.ExternalRoundId,
                dataType,
                agent.AgentId,
                agent.SecretKey).ConfigureAwait(false);

        return new RoundDetailResult
        {
            Data = new RoundDetailDataBase[]
            {
                new RoundDetailDataUrl
                {
                    Url = url??string.Empty
                }
            }
        };
    }
}