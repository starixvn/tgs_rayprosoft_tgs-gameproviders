using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Saba.Models;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders.Core.Saba.ApplicationServices
{
    internal class SabaRoundDetailGetService : IGetService<SabaRoundDetailQuery, RoundDetailResult>
    {
        private readonly ISabaApiClient _client;
        private readonly IGameActionService _gameActionService;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILogger<SabaRoundDetailGetService> _logger;
        private readonly IRoundService _roundService;
        private readonly ISabaService _service;

        public SabaRoundDetailGetService(
            ILogger<SabaRoundDetailGetService> logger,
            IRoundService roundService,
            IGameActionService gameActionService,
            ISabaService service,
            ISabaApiClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
            _gameActionService = gameActionService ?? throw new ArgumentNullException(nameof(gameActionService));
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
            var roundInfo = await _roundService.GetAsync(query.RoundId).ConfigureAwait(false);
            if (roundInfo == null)
            {
                throw new Exception($"This game round number could not be found.{query.RoundId}");
            }

            var gameActionInfo = await _gameActionService
                .QueryAsync(query.RoundId)
                .FirstOrDefaultAsync(ga => ga.Type == GameActionType.PlaceBet, cancellationToken)
                .ConfigureAwait(false);

            if (gameActionInfo == null)
            {
                throw new Exception($"The place bet game action of given round id could not be found. {roundInfo.RoundId}");
            }

            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            var request = new SabaGetRoundHistoryRequest
            {
                VendorId = configuration.VendorId,
                ExternalTransactionId = gameActionInfo.ExternalTransactionId
            };

            var response = await _client.GetBetDetailByTransIdAsync(configuration.ApiUrl, request).ConfigureAwait(false);

            var value = default(string);

            if (response.ErrorCode == SabaGetRoundHistoryApiError.Ok && response.Data?.Results?.Length > 0)
            {
                value = JsonSerializer.Serialize(response.Data.Results, _jsonSerializerOptions);
            }

            return new RoundDetailResult
            {
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
