using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.LY.Models;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains;

namespace TGS.GameProviders.Core.LY.ApplicationServices
{
    public class LYGameUrlGetService : IGetService<LYGameUrlQuery, GameUrlResult>
    {
        private readonly ILYApiClient _client;
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<LYGameUrlGetService> _logger;
        private readonly ILYService _service;

        public LYGameUrlGetService(
            ILYApiClient client,
            IGameRepository gameRepository,
            ILogger<LYGameUrlGetService> logger,
            ILYService service
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<GameUrlResult> GetAsync(LYGameUrlQuery command, CancellationToken cancellationToken = default)
        {
            var brandMerchant = await _service.GetBrandMerchantAsync(command.BrandCode).ConfigureAwait(false);

            var agentId = await _service.GetAgentIdAsync(command.BrandCode, command.CurrencyCode).ConfigureAwait(false);

            var externalID = 0;

            if (command.GameType != GameType.CardGameLobby)
            {
                var getGame = await _gameRepository.GetGameAsync(command.GameId.Value).ConfigureAwait(false);

                if (getGame != null)
                {
                    if (getGame.Type != GameType.CardGameLobby)
                    {
                        externalID = Convert.ToInt32(getGame.ExternalId);
                    }
                }
            }

            var dateTimeOffset = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));

            var orderId = $"{agentId}{dateTimeOffset.ToString("yyyyMMddHHmmssfff")}{command.PlayerCode}";

            var data = new LYGameUrlRequest
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

            return (response != null && response?.Data.Code == 0) ? new() { Url = new(response.Data.Url) } : null;
        }
    }
}
