using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.KY.Models;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Domains;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Core.KY.ApplicationServices
{
    public class KYGameUrlGetService : IGetService<KYGameUrlQuery, KYGameUrlResult>
    {
        private readonly IKYApiClient _client;
        private readonly ILogger<KYGameUrlGetService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly IKYService _service;
        private readonly IGameRepository _gameRepository;

        public KYGameUrlGetService(
            ILogger<KYGameUrlGetService> logger,
            IKYApiClient client,
            IKYService service,
            IPlayerRepository playerRepository,
            IGameRepository gameRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        public async ValueTask<KYGameUrlResult> GetAsync(KYGameUrlQuery command, CancellationToken cancellationToken = default)
        {
            var brandMerchant = await _service.GetBrandMerchantAsync(command.BrandCode).ConfigureAwait(false);

            var agentId = await _service.GetAgentIdAsync(command.BrandCode, command.CurrencyCode).ConfigureAwait(false);

            var externalID = 0;

            if (command.GameType != GameType.CardGameLobby)
            {

                var cachedGame = await _gameRepository.GetGameAsync(command.GameId.Value).ConfigureAwait(false);

                if (cachedGame != null)
                {
                    if (cachedGame.Type != GameType.CardGameLobby)
                    {
                        externalID = Convert.ToInt32(cachedGame.ExternalId);
                    }
                }
            }

            var dateTimeOffset = DateTimeOffset.UtcNow.AddHours(+8);

            var orderId = $"{agentId}{dateTimeOffset.ToString("yyyyMMddHHmmssfff")}{command.PlayerCode}";

            var data = new KYGameUrlRequest
            {
                PlayerCode = command.PlayerCode,
                OrderId = orderId,
                Amount = 0m,
                IP = command.IPAddress,
                BrandsCode = command.BrandCode,
                ExternalID = externalID
            };

            var response = await _client.GetGameUrlAsync(brandMerchant.ApiUrl, brandMerchant.Deskey, brandMerchant.Md5key, agentId, dateTimeOffset, data)
                .ConfigureAwait(false);

            return (response.Data.Code == 0) ? new() { Url = new(response.Data.Url) } : null;
        }
    }
}
