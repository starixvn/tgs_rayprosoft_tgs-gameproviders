using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Evolution.Models;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders.Core.Evolution.ApplicationServices
{
    internal class EvolutionRoundDetailGetService : IGetService<EvolutionRoundDetailQuery, RoundDetailResult>
    {
        private readonly IEvolutionApiClient _client;
        private readonly IEvolutionRoundDetailAnalyzeService _evolutionRoundDetailAnalyzeService;
        private readonly IGameService _gameService;
        private readonly IRoundService _roundService;
        private readonly IEvolutionService _service;

        public EvolutionRoundDetailGetService(
          IEvolutionApiClient client,
          IEvolutionRoundDetailAnalyzeService evolutionRoundDetailAnalyzeService,
          IGameService gameService,
          IRoundService roundService,
          IEvolutionService service
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _evolutionRoundDetailAnalyzeService = evolutionRoundDetailAnalyzeService ?? throw new ArgumentNullException(nameof(evolutionRoundDetailAnalyzeService));
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<RoundDetailResult> GetAsync(EvolutionRoundDetailQuery query, CancellationToken cancellationToken = default)
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

            return gameInfo.GameType == GameType.Slot
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
}
