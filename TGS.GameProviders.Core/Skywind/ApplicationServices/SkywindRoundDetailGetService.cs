using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Skywind.Models;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders.Core.Skywind.ApplicationServices
{
    internal class SkywindRoundDetailGetService : IGetService<SkywindRoundDetailQuery, RoundDetailResult>
    {
        private readonly ISkywindApiClient _client;
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameService _gameService;
        private readonly IRoundService _roundService;
        private readonly ISkywindService _service;
        private readonly ISkywindAccessToken _skywindAccessToken;

        public SkywindRoundDetailGetService(
            ISkywindApiClient client,
            IGameProviderRepository gameProviderRepository,
            IGameService gameService,
            IRoundService roundService,
            ISkywindService service,
            ISkywindAccessToken skywindAccessToken
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _skywindAccessToken = skywindAccessToken ?? throw new ArgumentNullException(nameof(skywindAccessToken));
        }

        public async ValueTask<RoundDetailResult> GetAsync(SkywindRoundDetailQuery query, CancellationToken cancellationToken = default)
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

            var roundId = roundModel.ExternalRoundId;
            if (roundId.IndexOf("_") != -1)
            {
                roundId = roundId.Split("_")[0];
            }

            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            var getaccessToken = await _skywindAccessToken.GetAccessTokenAsync().ConfigureAwait(false);
            var response = await _client
                .GetGamHistoryRoundDetailsAsync(
                configuration.ApiUrl,
                getaccessToken,
                new SkywindGamHistoryRoundDetailsRequest
                {
                    RoundId = roundId
                }).ConfigureAwait(false);

            return new()
            {
                Data = new RoundDetailDataBase[]
                {
                    new RoundDetailDataUrl { Url = response.ImageUrl }
                }
            };
        }
    }
}
