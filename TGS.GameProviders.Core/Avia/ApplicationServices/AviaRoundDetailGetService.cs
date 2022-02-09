using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Avia.Models;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders.Core.Avia.ApplicationServices
{
    internal class AviaRoundDetailGetService : IGetService<AviaRoundDetailQuery, RoundDetailResult>
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameService _gameService;
        private readonly IRoundService _roundService;

        public AviaRoundDetailGetService(
            IGameProviderRepository gameProviderRepository,
            IGameService gameService,
            IRoundService roundService)
        {
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
        }

        public async ValueTask<RoundDetailResult> GetAsync(AviaRoundDetailQuery query, CancellationToken cancellationToken = default)
        {
            var roundModel = await _roundService.GetAsync(query.RoundId).ConfigureAwait(false);
            if (roundModel == null)
            {
                throw new Exception($"This game round number could not be found.{query.RoundId}");
            }

            return new()
            {
                Data = new RoundDetailDataBase[]
                {
                    new RoundDetailDataJson {Value = roundModel.AdditionalData}
                }
            };
        }
    }
}
