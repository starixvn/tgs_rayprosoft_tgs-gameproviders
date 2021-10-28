using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.Repositories;

namespace TGS.GameProviders.Core.BigGaming.ApplicationServices
{
    public class BigGamingGameUrlGetService : IGetService<BigGamingGameUrlQuery, GameUrlResult>
    {
        private readonly IBigGamingApiClient _client;
        private readonly IGameRepository _gameRepository;
        private readonly IBigGamingGameUrl _gameUrl;
        private readonly IBigGamingGamePlayer _gamingGamePlayerCreate;
        private readonly ILogger<BigGamingGameUrlGetService> _logger;
        private readonly IBigGamingService _service;

        public BigGamingGameUrlGetService(
            IBigGamingApiClient client,
            IGameRepository gameRepository,
            ILogger<BigGamingGameUrlGetService> logger,
            IBigGamingService service,
            IBigGamingGamePlayer gamingGamePlayerCreate,
            IBigGamingGameUrl gameUrl
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _gamingGamePlayerCreate = gamingGamePlayerCreate ?? throw new ArgumentNullException(nameof(gamingGamePlayerCreate));
            _gameUrl = gameUrl ?? throw new ArgumentNullException(nameof(gameUrl));
        }

        public async ValueTask<GameUrlResult> GetAsync(BigGamingGameUrlQuery command, CancellationToken cancellationToken = default)
        {
            var gameProviderCultureCode = await _service.GetGameProviderCultureCodeAsync(command.Language).ConfigureAwait(false);
            var brandMerchant = await _service.GetBrandMerchantAsync(command.CurrencyCode).ConfigureAwait(false);

            if (brandMerchant == null)
            {
                return null;
            }

            var getGame = await _gameRepository.GetGameAsync(command.GameId.Value).ConfigureAwait(false);

            if (getGame == null)
            {
                return null;
            }

            var responseGameUrl = await _gameUrl.GetGameUrlAsync(brandMerchant, command.PlayerCode, getGame, gameProviderCultureCode).ConfigureAwait(false);

            if (responseGameUrl.Error != null)
            {
                if (responseGameUrl.Error.Code == "2213")
                {
                    if (await _gamingGamePlayerCreate.CreatePlayerAsync(command.BrandCode, command.PlayerCode, command.CurrencyCode).ConfigureAwait(false))
                    {
                        responseGameUrl = await _gameUrl.GetGameUrlAsync(brandMerchant, command.PlayerCode, getGame, gameProviderCultureCode).ConfigureAwait(false);
                    }
                }
            }

            if (responseGameUrl.Error == null)
            {
                var url = JsonSerializer.Deserialize<string>(JsonSerializer.Serialize(responseGameUrl.Result));
            }
            return (responseGameUrl.Error == null) ? new() { Url = new Uri(JsonSerializer.Deserialize<string>(JsonSerializer.Serialize(responseGameUrl.Result))) } : null;
        }
    }
}
