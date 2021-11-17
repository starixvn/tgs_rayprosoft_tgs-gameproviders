using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories;

namespace TGS.GameProviders.Core.VR.ApplicationServices
{
    public class VRGameUrlGetService : IGetService<VRGameUrlQuery, GameUrlResult>
    {
        private readonly IVRApiClient _client;
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<VRGameUrlGetService> _logger;
        private readonly IVRService _service;

        public VRGameUrlGetService(
            IVRApiClient client,
            IGameRepository gameRepository,
            ILogger<VRGameUrlGetService> logger,
            IVRService service
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<GameUrlResult> GetAsync(VRGameUrlQuery command, CancellationToken cancellationToken = default)
        {
            var brandMerchant = await _service.GetBrandMerchantAsync(command.BrandCode).ConfigureAwait(false);

            if (brandMerchant == null)
            {
                return null;
            }

            var response = await _client.GetBalanceAsync(
                brandMerchant.ApiUrl,
                brandMerchant.Merchantid,
                brandMerchant.Version,
                brandMerchant.Deskey,
                new()
                {
                    PlayerCode = command.PlayerCode
                }).ConfigureAwait(false);

            if (string.IsNullOrEmpty(response.PlayerName) || response.Balance == -1)
            {
                return null;
            }

            var date = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
            var externalID = 0;
            if (command.GameId > 0)
            {
                var getGame = await _gameRepository.GetAsync(command.GameId.Value).ConfigureAwait(false);

                if (getGame != null)
                {
                    externalID = Convert.ToInt32(getGame.GameExternalId);
                }
            }

            return new()
            {
                Url =
                _client.GetGameURL(
                brandMerchant.ApiUrl,
                brandMerchant.Merchantid,
                brandMerchant.Version,
                brandMerchant.Deskey,
                new()
                {
                    PlayerCode = command.PlayerCode,
                    LoginTime = date.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    IsCreateUser = "true",
                    ChannelId = externalID,
                    ChannelType = 0
                })
            };
        }
    }
}
