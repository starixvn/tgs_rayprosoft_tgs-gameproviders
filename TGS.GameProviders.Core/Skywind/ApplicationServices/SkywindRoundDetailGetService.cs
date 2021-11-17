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
        private readonly IGameRepository _gameRepository;
        private readonly IRoundRepository _roundRepository;
        private readonly ISkywindService _service;
        private readonly ISkywindAccessToken _skywindAccessToken;

        public SkywindRoundDetailGetService(
            ISkywindApiClient client,
            IGameProviderRepository gameProviderRepository,
            IGameRepository gameRepository,
            IRoundRepository roundRepository,
            ISkywindService service,
            ISkywindAccessToken skywindAccessToken
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _skywindAccessToken = skywindAccessToken ?? throw new ArgumentNullException(nameof(skywindAccessToken));
        }

        public async ValueTask<RoundDetailResult> GetAsync(SkywindRoundDetailQuery query, CancellationToken cancellationToken = default)
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
                GameProviderName = gameProviderModel.Name,
                GameName = gameModel.Name,
                ExternalRoundId = roundModel.ExternalRoundId,
                BetAmount = roundModel.BetAmount,
                Turnover = roundModel.Turnover,
                Winloss = roundModel.WinLoss,
                GameResult = roundModel.ClientStatus.ToString(),
                Data = new RoundDetailDataBase[]
                {
                    new RoundDetailDataUrl { Url = response.ImageUrl }
                }
            };
        }
    }
}
