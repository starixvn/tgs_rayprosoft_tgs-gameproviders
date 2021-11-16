using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Evolution.Models;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders.Core.Evolution.ApplicationServices
{
    internal class EvolutionRoundDetailGetService : IGetService<EvolutionRoundDetailQuery, RoundDetailResult>
    {
        private readonly IRoundRepository _roundRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IEvolutionService _service;
        private readonly IEvolutionApiClient _client;
        private readonly IEvolutionRoundDetailAnalyzeService _evolutionRoundDetailAnalyzeService;
        public EvolutionRoundDetailGetService(
            IEvolutionApiClient client,
            IRoundRepository roundRepository,
            IGameRepository gameRepository,
            IEvolutionService service,
            IEvolutionRoundDetailAnalyzeService evolutionRoundDetailAnalyzeService)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _evolutionRoundDetailAnalyzeService = evolutionRoundDetailAnalyzeService ?? throw new ArgumentNullException(nameof(evolutionRoundDetailAnalyzeService));
        }
        public async ValueTask<RoundDetailResult> GetAsync(EvolutionRoundDetailQuery query, CancellationToken cancellationToken = default)
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

            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);


            var response = await _client.GetGamHistoryRoundAsync(
                configuration.ApiUrl,
                configuration.CasinoID,
                configuration.GameHistoryApiToken,
                new EvolutionGamHistoryRoundRequest
                {
                    PlayerCode = round.PlayerCode,
                    RoundId = round.ExternalRoundId
                }).ConfigureAwait(false);

            if (game.Type == GameType.Slot)
            {
                return new RoundDetailResult
                {
                    BetAmount = round.BetAmount ?? 0,
                    Turnover = round.Turnover ?? 0,
                    Winloss = round.WinLoss ?? 0,
                    GameResult = round.ClientStatus.ToString(),
                    Data = new RoundDetailDataBase[]
                    {
                        new RoundDetailDataUrl {Url = _evolutionRoundDetailAnalyzeService.GetAnalyzeSlot(configuration.ApiUrl,response)}
                    }
                };
            }

            return new RoundDetailResult
            {
                BetAmount = round.BetAmount ?? 0,
                Turnover = round.Turnover ?? 0,
                Winloss = round.WinLoss ?? 0,
                GameResult = round.ClientStatus.ToString(),
                Data = new RoundDetailDataBase[]
                {
                    new RoundDetailDataJson {Value = _evolutionRoundDetailAnalyzeService.GetAnalyze(response)}
                }
            };
        }
    }
}
