using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.SexyGaming.Models;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SexyGaming.ApplicationServices
{
    internal class SexyGamingRoundDetailGetService : IGetService<SexyGamingRoundDetailQuery, RoundDetailResult>
    {
        private readonly IGameService _gameService;
        private readonly IRoundService _roundService;
        private readonly ISexyGamingRoundDetailAnalyzeService _sexyGamingRoundDetailAnalyzeService;

        public SexyGamingRoundDetailGetService(
            IGameService gameService,
            IRoundService roundService,
            ISexyGamingRoundDetailAnalyzeService sexyGamingRoundDetailAnalyzeService
            )
        {
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
            _sexyGamingRoundDetailAnalyzeService = sexyGamingRoundDetailAnalyzeService ?? throw new ArgumentNullException(nameof(sexyGamingRoundDetailAnalyzeService));
        }

        public async ValueTask<RoundDetailResult> GetAsync(SexyGamingRoundDetailQuery query, CancellationToken cancellationToken = default)
        {
            var roundInfo = await _roundService.GetAsync(query.RoundId).ConfigureAwait(false);
            if (roundInfo == null)
            {
                throw new Exception($"This game round number could not be found.{query.RoundId}");
            }

            var gameInfo = await _gameService.GetCacheAsync(roundInfo.GameId).ConfigureAwait(false);
            if (gameInfo == null)
            {
                throw new Exception($"This game Id could not be found..{roundInfo.GameId}");
            }

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
}
