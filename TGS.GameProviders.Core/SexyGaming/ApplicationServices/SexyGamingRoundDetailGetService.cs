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
        private readonly IGameService _gameService;
        private readonly IRoundService _roundService;
        private readonly ISexyGamingRoundDetailAnalyzeService _sexyGamingRoundDetailAnalyzeService;

        public SexyGamingRoundDetailGetService(
            IGameProviderRepository gameProviderRepository,
            IGameService gameService,
            IRoundService roundService,
            ISexyGamingRoundDetailAnalyzeService sexyGamingRoundDetailAnalyzeService
            )
        {
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
            _sexyGamingRoundDetailAnalyzeService = sexyGamingRoundDetailAnalyzeService ?? throw new ArgumentNullException(nameof(sexyGamingRoundDetailAnalyzeService));
        }

        public async ValueTask<RoundDetailResult> GetAsync(SexyGamingRoundDetailQuery query, CancellationToken cancellationToken = default)
        {
            var roundModel = await _roundService.GetAsync(query.RoundId).ConfigureAwait(false);
            if (roundModel == null)
            {
                throw new Exception($"This game round number could not be found.{query.RoundId}");
            }
            var gameProviderModel = await _gameProviderRepository.GetAsync(roundModel.GameProviderId).ConfigureAwait(false);
            if (gameProviderModel == null)
            {
                throw new Exception($"This game manufacturer does not exist.{roundModel.GameProviderId}");
            }

            var gameModel = await _gameService.GetCacheAsync(roundModel.GameId).ConfigureAwait(false);
            if (gameModel == null)
            {
                throw new Exception($"This game Id could not be found..{gameModel.GameId}");
            }

            var gameRecord = JsonSerializer.Deserialize<SexyGamingGameRecord>(roundModel.AdditionalData);

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
                        new RoundDetailDataJson {Value = _sexyGamingRoundDetailAnalyzeService.GetAnalyze(gameModel.GameExternalId, gameRecord)}
                    }
                });
        }
    }
}
