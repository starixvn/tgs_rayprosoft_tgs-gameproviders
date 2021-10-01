using System;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Domains.Models.Rounds;

namespace TGS.GameProviders.Core.GameRecordProcessor
{
    internal sealed class GameRecordProcessor : IGameRecordProcessor
    {
        private readonly IGameRecordProcessorFailed _gameRecordProcessorFailed;
        private readonly IGameRecordProcessorSuccess _gameRecordProcessorSuccess;
        private readonly IGameRepository _gameRepository;
        private readonly IRoundRepository _roundRepository;

        public GameRecordProcessor(
            IRoundRepository roundRepository,
            IGameRepository gameRepository,
            IGameRecordProcessorSuccess gameRecordProcessorSuccess,
            IGameRecordProcessorFailed gameRecordProcessorFailed)
        {
            _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _gameRecordProcessorSuccess = gameRecordProcessorSuccess ?? throw new ArgumentNullException(nameof(gameRecordProcessorSuccess));
            _gameRecordProcessorFailed = gameRecordProcessorFailed ?? throw new ArgumentNullException(nameof(gameRecordProcessorFailed));
        }

        public async ValueTask<bool> ExecuteAsync(GameRoundsData data)
        {
            if (data.Success)
            {
                await _gameRecordProcessorSuccess.ProcessorAsync(data).ConfigureAwait(false);
            }
            else
            {
                if (data.Status == RoundStatus.Closed)
                {
                    await _gameRecordProcessorFailed.ProcessorAsync(data).ConfigureAwait(false);
                }
            }

            return true;
        }

        public Task<GetRound> GetAsync(Guid roundId)
            => _roundRepository.GetRoundAsync(roundId);

        public Task<GetRound> GetExternalRoundIdAsync(Guid gameProviderId, string externalRoundId, int gameId, string playerCode)
            => _roundRepository.GetRoundAsync(gameProviderId, externalRoundId, gameId, playerCode);

        public async Task<GetRound> GetExternalRoundIdByExternalGameIdAsync(Guid gameProviderId, string externalRoundId, string externalGameId, string playerCode)
        {
            var game = await _gameRepository.GetGameAsync(gameProviderId, externalGameId).ConfigureAwait(false);
            return await _roundRepository.GetRoundAsync(gameProviderId, externalRoundId, game.Id, playerCode).ConfigureAwait(false);
        }

        public Task UpdateRoundDetailAsync(Guid roundId, string additionalData)
            => _roundRepository.UpdateRoundDetailAsync(roundId, additionalData);
    }
}
