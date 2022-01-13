using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Core.Infrastructures.Security;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories;
using System.Linq;

namespace TGS.GameProviders.Core.TFGaming.ApplicationServices
{
    public class TFGamingGameUrlGetService : IGetService<TFGamingGameUrlQuery, GameUrlResult>
    {
        private readonly ITFApiClient _client;
        private readonly ITFGamingService _service;
        private readonly IPlayerRepository _playerRepository;

        public TFGamingGameUrlGetService(
            ITFApiClient client,
            ITFGamingService service,
            IPlayerRepository playerRepository)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        public async ValueTask<GameUrlResult> GetAsync(TFGamingGameUrlQuery query, CancellationToken cancellationToken = default)
        {            
            var player = await _playerRepository.GetAsync(query.PlayerCode).ConfigureAwait(false);

            var brandCode = player.BrandCode;
            var currencyCode = player.DefaultWalletCurrencyCode;

            var brandMerchant = await _service.GetBrandMerchantAsync(brandCode, currencyCode).ConfigureAwait(false);



            var url = await _client
                .GetGameUrlAsync(brandMerchant.LaunchUrl,brandMerchant.PublicToken, query.PlayerCode)
                .ConfigureAwait(false);

            return new GameUrlResult
            {
                Url = url
            };
        }
    }
}