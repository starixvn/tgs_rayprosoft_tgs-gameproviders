using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core.Evolution.Models;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Evolution.ApplicationServices;

internal class EvolutionRoundDetailGetService : IGetService<EvolutionRoundDetailQuery, RoundDetailResult>
{
    private readonly IEvolutionApiClient _client;
    private readonly IEvolutionRoundDetailAnalyzeService _evolutionRoundDetailAnalyzeService;
    private readonly IGameRepository _gameRepository;
    private readonly IRoundRepository _roundService;
    private readonly IEvolutionService _service;

    public EvolutionRoundDetailGetService(
      IEvolutionApiClient client,
      IEvolutionRoundDetailAnalyzeService evolutionRoundDetailAnalyzeService,
      IGameRepository gameRepository,
      IRoundRepository roundService,
      IEvolutionService service
        )
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _evolutionRoundDetailAnalyzeService = evolutionRoundDetailAnalyzeService ?? throw new ArgumentNullException(nameof(evolutionRoundDetailAnalyzeService));
        _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask<RoundDetailResult> GetAsync(EvolutionRoundDetailQuery query, CancellationToken cancellationToken = default)
    {
        var roundInfo = await _roundService
            .GetAsync(
            roundId: query.RoundId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameInfo = await _gameRepository
            .GetAsync(
            gameId: roundInfo.GameId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

        var response = await _client.GetGamHistoryRoundAsync(
            configuration.ApiUrl,
            configuration.CasinoID,
            configuration.GameHistoryApiToken,
            new EvolutionGamHistoryRoundRequest
            {
                PlayerCode = roundInfo.PlayerCode,
                RoundId = roundInfo.ExternalRoundId
            }).ConfigureAwait(false);

        return gameInfo.Type == GameType.Slot
            ? new RoundDetailResult
            {
                Data = new RoundDetailDataBase[]
                {
                    new RoundDetailDataUrl {Url = _evolutionRoundDetailAnalyzeService.GetAnalyzeSlot(configuration.ApiUrl,response)}
                }
            }
            : new RoundDetailResult
            {
                Data = new RoundDetailDataBase[]
            {
                new RoundDetailDataJson {Value = _evolutionRoundDetailAnalyzeService.GetAnalyze(response)}
            }
            };
    }
}
