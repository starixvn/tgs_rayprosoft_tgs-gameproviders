using System;
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
        private readonly IEvolutionApiClient _client;
        private readonly IEvolutionRoundDetailAnalyzeService _evolutionRoundDetailAnalyzeService;
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IRoundRepository _roundRepository;
        private readonly IEvolutionService _service;

        public EvolutionRoundDetailGetService(
          IEvolutionApiClient client,
          IEvolutionRoundDetailAnalyzeService evolutionRoundDetailAnalyzeService,
          IGameProviderRepository gameProviderRepository,
          IGameRepository gameRepository,
          IRoundRepository roundRepository,
          IEvolutionService service
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _evolutionRoundDetailAnalyzeService = evolutionRoundDetailAnalyzeService ?? throw new ArgumentNullException(nameof(evolutionRoundDetailAnalyzeService));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<RoundDetailResult> GetAsync(EvolutionRoundDetailQuery query, CancellationToken cancellationToken = default)
        {
            var roundModel = await _roundRepository.GetAsync(query.RoundId).ConfigureAwait(false);
            if (roundModel == null)
            {
                throw new Exception($"This game round number could not be found.{query.RoundId}");
            }
            var gameProviderModel = await _gameProviderRepository.GetAsync(roundModel.GameProviderId).ConfigureAwait(false);
            if (gameProviderModel == null)
            {
                throw new Exception($"This game manufacturer does not exist.{roundModel.GameProviderId}");
            }

            var gameModel = await _gameRepository.GetAsync(roundModel.GameId).ConfigureAwait(false);
            if (gameModel == null)
            {
                throw new Exception($"This game Id could not be found..{gameModel.GameId}");
            }

            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            var response = await _client.GetGamHistoryRoundAsync(
                configuration.ApiUrl,
                configuration.CasinoID,
                configuration.GameHistoryApiToken,
                new EvolutionGamHistoryRoundRequest
                {
                    PlayerCode = roundModel.PlayerCode,
                    RoundId = roundModel.ExternalRoundId
                }).ConfigureAwait(false);

            return gameModel.GameType == GameType.Slot
                ? new RoundDetailResult
                {
                    GameProviderName = gameProviderModel.Name,
                    GameName = gameModel.Name,
                    ExternalRoundId = roundModel.ExternalRoundId,
                    BetAmount = roundModel.BetAmount,
                    Turnover = roundModel.Turnover,
                    Winloss = roundModel.WinLoss,
                    GameResult = roundModel.ClientStatus.ToString(),
                    Data = new RoundDetailDataBase[]
                    {
                        new RoundDetailDataUrl {Url = _evolutionRoundDetailAnalyzeService.GetAnalyzeSlot(configuration.ApiUrl,response)}
                    }
                }
                : new RoundDetailResult
                {
                    GameProviderName = gameProviderModel.Name,
                    GameName = gameModel.Name,
                    ExternalRoundId = roundModel.ExternalRoundId,
                    BetAmount = roundModel.BetAmount,
                    Turnover = roundModel.Turnover,
                    Winloss = roundModel.WinLoss,
                    GameResult = roundModel.ClientStatus.ToString(),
                    Data = new RoundDetailDataBase[]
                {
                    new RoundDetailDataJson {Value = _evolutionRoundDetailAnalyzeService.GetAnalyze(response)}
                }
                };
        }
    }
}
