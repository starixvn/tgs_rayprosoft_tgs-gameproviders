using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.SexyGaming.Models;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SexyGaming.ApplicationServices;

internal class SexyGamingRoundDetailGetService : IGetService<SexyGamingRoundDetailQuery, RoundDetailResult>
{
    private readonly IGameRepository _gameRepository;
    private readonly IRoundRepository _roundService;
    private readonly ISexyGamingRoundDetailAnalyzeService _sexyGamingRoundDetailAnalyzeService;

    public SexyGamingRoundDetailGetService(
        IGameRepository gameRepository,
        IRoundRepository roundService,
        ISexyGamingRoundDetailAnalyzeService sexyGamingRoundDetailAnalyzeService
        )
    {
        _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
        _sexyGamingRoundDetailAnalyzeService = sexyGamingRoundDetailAnalyzeService ?? throw new ArgumentNullException(nameof(sexyGamingRoundDetailAnalyzeService));
    }

    public async ValueTask<RoundDetailResult> GetAsync(SexyGamingRoundDetailQuery query, CancellationToken cancellationToken = default)
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

        var gameRecord = JsonSerializer.Deserialize<SexyGamingGameRecord>(roundInfo.AdditionalData);

        return gameRecord == null
            ? (new()
            {
                Data = new RoundDetailDataBase[]
                {
                    new RoundDetailDataJson {Value = null}
                }
            })
            : (new()
            {
                Data = new RoundDetailDataBase[]
                {
                    new RoundDetailDataJson {Value = _sexyGamingRoundDetailAnalyzeService.GetAnalyze(gameInfo.GameExternalId, gameRecord)}
                }
            });
    }
}
