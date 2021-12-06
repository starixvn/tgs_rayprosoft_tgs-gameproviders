using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Saba.Models;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders.Core.Saba.ApplicationServices
{
    internal class SabaRoundDetailGetService : IGetService<SabaRoundDetailQuery, RoundDetailResult>
    {
        private readonly ISabaApiClient _client;
        private readonly IGameActionRepository _gameActionRepository;
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameRepository _gameRepository;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILogger<SabaRoundDetailGetService> _logger;
        private readonly IRoundRepository _roundRepository;
        private readonly ISabaService _service;

        public SabaRoundDetailGetService(
            ILogger<SabaRoundDetailGetService> logger,
            IGameProviderRepository gameProviderRepository,
            IGameRepository gameRepository,
            IRoundRepository roundRepository,
            IGameActionRepository gameActionRepository,
            ISabaService service,
            ISabaApiClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
            _gameActionRepository = gameActionRepository ?? throw new ArgumentNullException(nameof(gameActionRepository));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _client = client ?? throw new ArgumentNullException(nameof(client));

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = false
            };
        }

        public async ValueTask<RoundDetailResult> GetAsync(
            SabaRoundDetailQuery query,
            CancellationToken cancellationToken = default)
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
                throw new Exception($"This game Id could not be found.{roundModel.GameId}");
            }

            var gameActionModel = await _gameActionRepository
                .GetGameActionsByRoundIdAsync(query.RoundId)
                .FirstOrDefaultAsync(ga => ga.Type == GameActionType.PlaceBet, cancellationToken)
                .ConfigureAwait(false);

            if (gameActionModel == null)
            {
                throw new Exception($"The place bet game action of given round id could not be found. {roundModel.RoundId}");
            }

            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            var request = new SabaGetRoundHistoryRequest
            {
                VendorId = configuration.VendorId,
                ExternalTransactionId = gameActionModel.ExternalTransactionId
            };

            var response = await _client.GetBetDetailByTransIdAsync(configuration.ApiUrl, request).ConfigureAwait(false);

            var value = default(string);

            if (response.ErrorCode == SabaGetRoundHistoryApiError.Ok && response.Data?.Results?.Length > 0)
            {
                value = JsonSerializer.Serialize(response.Data.Results, _jsonSerializerOptions);
            }

            return new RoundDetailResult
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
                   new RoundDetailDataJson
                   {
                       Value = value
                   }
                }
            };
        }
    }
}