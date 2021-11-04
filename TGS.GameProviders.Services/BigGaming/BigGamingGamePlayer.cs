using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.BigGaming;
using TGS.GameProviders.Core.BigGaming.Models;
using TGS.GameProviders.Core.Extensions;

namespace TGS.GameProviders.Services.BigGaming
{
    internal class BigGamingGamePlayer : IBigGamingGamePlayer
    {
        private readonly IBigGamingApiClient _client;
        private readonly ILogger<BigGamingGamePlayer> _logger;
        private readonly IBigGamingService _service;

        public BigGamingGamePlayer(
            ILogger<BigGamingGamePlayer> logger,
            IBigGamingApiClient client,
            IBigGamingService service
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<bool> CreatePlayerAsync(string brandCode, string playerCode, string currencyCode)
        {
            var brandMerchant = await _service.GetBrandMerchantAsync(currencyCode).ConfigureAwait(false);

            var createPlayerRequest = new BigGamingCreatePlayerRequest()
            {
                Random = GuidExtensions.NewSequentialGuid().ToString(),
                Sn = brandMerchant.SecretCode,
                LoginId = playerCode,
                Nickname = playerCode,
                AgentLoginId = brandCode
            };

            var response = await _client.CreatePlayerAsync(brandMerchant.ApiUrl, brandMerchant.SecretCode, brandMerchant.AgentLoginIdPassword, createPlayerRequest).ConfigureAwait(false);

            return (response.Error == null);
        }
    }
}
