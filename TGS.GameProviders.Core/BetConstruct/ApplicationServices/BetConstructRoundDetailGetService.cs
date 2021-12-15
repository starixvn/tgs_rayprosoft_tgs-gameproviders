using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.BetConstruct.Models;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders.Core.BetConstruct.ApplicationServices
{
    internal class BetConstructRoundDetailGetService : IGetService<BetConstructRoundDetailQuery, RoundDetailResult>
    {
        private readonly IRoundRepository _roundRepository;
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameRepository _gameRepository;

        public BetConstructRoundDetailGetService(
            IRoundRepository roundRepository,
            IGameProviderRepository gameProviderRepository,
            IGameRepository gameRepository)
        {
            _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        public async ValueTask<RoundDetailResult> GetAsync(BetConstructRoundDetailQuery query, CancellationToken cancellationToken = default)
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

            var gameRecord = JsonSerializer.Deserialize<BetConstructGameRecord[]>(roundModel.AdditionalData);

            return gameRecord == null
                ? (new RoundDetailResult()
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
                    new RoundDetailDataJson {Value = JsonSerializer.Serialize(gameRecord)}
                }
                });
        }
    }
}