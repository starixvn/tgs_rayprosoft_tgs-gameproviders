using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.TFGaming.ApplicationServices
{
    public class TFGamingGameUrlGetService : IGetService<TFGamingGameUrlQuery, GameUrlResult>
    {
        private readonly ITFApiClient _client;
        private readonly IPlayerRepository _playerService;
        private readonly ITFGamingService _service;

        public TFGamingGameUrlGetService(
            ITFApiClient client,
            ITFGamingService service,
            IPlayerRepository playerService)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        }

        public async ValueTask<GameUrlResult> GetAsync(TFGamingGameUrlQuery query, CancellationToken cancellationToken = default)
        {
            var player = await _playerService.GetAsync(query.PlayerCode).ConfigureAwait(false);

            var brandCode = player.BrandCode;
            var currencyCode = player.DefaultWalletCurrencyCode;

            var brandMerchant = await _service.GetBrandMerchantAsync(brandCode, currencyCode).ConfigureAwait(false);

            var url = await _client
                .GetGameUrlAsync(brandMerchant.LaunchUrl, brandMerchant.PublicToken, query.PlayerCode)
                .ConfigureAwait(false);

            return new GameUrlResult
            {
                Url = url
            };
        }
    }
}
