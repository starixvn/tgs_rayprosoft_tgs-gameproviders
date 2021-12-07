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
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IRoundRepository _roundRepository;
        private readonly ISexyGamingRoundDetailAnalyzeService _sexyGamingRoundDetailAnalyzeService;

        public SexyGamingRoundDetailGetService(
            IGameProviderRepository gameProviderRepository,
            IGameRepository gameRepository,
            IRoundRepository roundRepository,
            ISexyGamingRoundDetailAnalyzeService sexyGamingRoundDetailAnalyzeService
            )
        {
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
            _sexyGamingRoundDetailAnalyzeService = sexyGamingRoundDetailAnalyzeService ?? throw new ArgumentNullException(nameof(sexyGamingRoundDetailAnalyzeService));
        }

        public async ValueTask<RoundDetailResult> GetAsync(SexyGamingRoundDetailQuery query, CancellationToken cancellationToken = default)
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

            var gameRecord = JsonSerializer.Deserialize<SexyGamingGameRecord>(roundModel.AdditionalData);

            return gameRecord == null
                ? (new()
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
                        new RoundDetailDataJson {Value = null}
                    }
                })
                : (new()
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
                        new RoundDetailDataJson {Value = _sexyGamingRoundDetailAnalyzeService.GetAnalyze(gameModel.GameExternalId, gameRecord)}
                    }
                });
        }
    }
}
