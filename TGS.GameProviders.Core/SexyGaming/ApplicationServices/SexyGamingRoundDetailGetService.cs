using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.SexyGaming.Models;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders.Core.SexyGaming.ApplicationServices
{
    internal class SexyGamingRoundDetailGetService : IGetService<SexyGamingRoundDetailQuery, RoundDetailResult>
    {
        private readonly IGameRepository _gameRepository;
        private readonly IRoundRepository _roundRepository;
        private readonly ISexyGamingRoundDetailAnalyzeService _sexyGamingRoundDetailAnalyzeService;

        public SexyGamingRoundDetailGetService(
            IGameRepository gameRepository,
            IRoundRepository roundRepository,
            ISexyGamingRoundDetailAnalyzeService sexyGamingRoundDetailAnalyzeService
            )
        {
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
            _sexyGamingRoundDetailAnalyzeService = sexyGamingRoundDetailAnalyzeService ?? throw new ArgumentNullException(nameof(sexyGamingRoundDetailAnalyzeService));
        }

        public async ValueTask<RoundDetailResult> GetAsync(SexyGamingRoundDetailQuery query, CancellationToken cancellationToken = default)
        {
            var round = await _roundRepository.GetRoundAsync(query.RoundId).ConfigureAwait(false);
            if (round == null)
            {
                throw new Exception($"This game round number could not be found.{query.RoundId}");
            }

            var game = await _gameRepository.GetGameAsync(round.GameId).ConfigureAwait(false);

            if (game == null)
            {
                throw new Exception($"This game Id could not be found..{round.GameId}");
            }

            var gameRecord = JsonSerializer.Deserialize<SexyGamingGameRecord>(round.AdditionalData);

            if (gameRecord == null)
            {
                return null;
            }

            return new()
            {
                BetAmount = round.BetAmount ?? 0,
                Turnover = round.Turnover ?? 0,
                Winloss = round.WinLoss ?? 0,
                GameResult = round.ClientStatus.ToString(),
                Data = new RoundDetailDataBase[]
                {
                    new RoundDetailDataJson {Value = _sexyGamingRoundDetailAnalyzeService.GetAnalyze(game.GameExternalId, gameRecord)}
                }
            };
        }
    }
}
