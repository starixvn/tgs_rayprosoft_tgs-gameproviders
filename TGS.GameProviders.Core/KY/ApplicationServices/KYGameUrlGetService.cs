using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace TGS.GameProviders.Core.KY.ApplicationServices
{
    public class KYGameUrlGetService : IGetService<KYGameUrlQuery, GameUrlResult>
    {
        private readonly IKYApiClient _client;
        private readonly IGameService _gameService;
        private readonly ILogger<KYGameUrlGetService> _logger;
        private readonly IKYService _service;

        public KYGameUrlGetService(
            IKYApiClient client,
            IGameService gameService,
            ILogger<KYGameUrlGetService> logger,
            IKYService service
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<GameUrlResult> GetAsync(KYGameUrlQuery command, CancellationToken cancellationToken = default)
        {
            var brandMerchant = await _service.GetBrandMerchantAsync(command.BrandCode).ConfigureAwait(false);

            var agentId = await _service.GetAgentIdAsync(command.BrandCode, command.CurrencyCode).ConfigureAwait(false);

            var gameExternalID = 0;

            if (command.GameType != GameType.CardGameLobby)
            {
                var getGame = await _gameService.GetCacheAsync(command.GameId.Value).ConfigureAwait(false);

                if (getGame != null)
                {
                    if (getGame.GameType != GameType.CardGameLobby)
                    {
                        gameExternalID = Convert.ToInt32(getGame.GameExternalId);
                    }
                }
            }

            var response = await _client.GetGameUrlAsync(
                brandMerchant.ApiUrl,
                brandMerchant.Deskey,
                brandMerchant.Md5key,
                agentId,
                command.PlayerCode,
                command.IPAddress,
                command.BrandCode,
                gameExternalID).ConfigureAwait(false);

            return (response != null) ? new() { Url = response } : null;
        }
    }
}
