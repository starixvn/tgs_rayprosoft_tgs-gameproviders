using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace TGS.GameProviders.Core.Avia.ApplicationServices
{
    public class AviaGameUrlGetService : IGetService<AviaGameUrlQuery, GameUrlResult>
    {
        private readonly IAviaApiClient _client;
        private readonly IAviaGamePlayerService _gamePlayerService;
        private readonly ILogger<AviaGameUrlGetService> _logger;
        private readonly IAviaService _service;

        public AviaGameUrlGetService(
            IAviaApiClient client,
            ILogger<AviaGameUrlGetService> logger,
            IAviaService service,
            IAviaGamePlayerService gamePlayerService
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _gamePlayerService = gamePlayerService ?? throw new ArgumentNullException(nameof(gamePlayerService));
        }

        public async ValueTask<GameUrlResult> GetAsync(AviaGameUrlQuery command, CancellationToken cancellationToken = default)
        {
            var brandMerchant = await _service.GetBrandMerchantAsync(command.BrandCode);

            if (brandMerchant == null)
            {
                throw new ArgumentNullException(nameof(brandMerchant));
            }

            var gameProviderCurrencyCode = await _service.GetGameProviderCurrencyCodeAsync(command.CurrencyCode).ConfigureAwait(false);
            var gameProviderCultureCode = await _service.GetGameProviderCultureCodeAsync(command.Language).ConfigureAwait(false);

            (bool IsPlayer, Uri? GameUrl) = await _client.GetGameUrlAsync(
                brandMerchant.ApiUrl,
                brandMerchant.Authorization,
                command.PlayerCode,
                gameProviderCultureCode,
                null,
                null).ConfigureAwait(false);

            if (!IsPlayer)
            {
                var createPlayerResult = await _gamePlayerService.CreatePlayerAsync(
                    brandMerchant,
                    command.PlayerCode,
                    gameProviderCurrencyCode).ConfigureAwait(false);
                if (createPlayerResult)
                {
                    (IsPlayer, GameUrl) = await _client.GetGameUrlAsync(
                        brandMerchant.ApiUrl,
                        brandMerchant.Authorization,
                        command.PlayerCode,
                        gameProviderCultureCode,
                        null,
                        null).ConfigureAwait(false);
                }
            }

            return GameUrl != null ? new() { Url = GameUrl } : null;
        }
    }
}
