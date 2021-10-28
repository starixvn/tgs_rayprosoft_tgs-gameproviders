using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.BigGaming.Models;
using TGS.GameProviders.ApiClients.BigGaming.Models.Commons;
using TGS.GameProviders.Domains.Extensions;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Domains.Models.Game;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.BigGaming;

namespace TGS.GameProviders.Core.BigGaming
{
    internal class BigGamingGameUrl : IBigGamingGameUrl
    {
        private readonly IBigGamingApiClient _client;
        private readonly ILogger<BigGamingGameUrl> _logger;
        private readonly IBigGamingService _service;

        public BigGamingGameUrl(
                     IBigGamingApiClient client,
                     ILogger<BigGamingGameUrl> logger,
                     IBigGamingService service
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<BigGamingResponse> GetGameUrlAsync(BigGamingBrandMerchant brandMerchant, string playerCode, GetGame game, string cultureCode) => game.Type
          switch
        {
            GameType.Fishing => await GetGameUrlFishingAsync(brandMerchant, playerCode, game.ExternalId, cultureCode).ConfigureAwait(false),
            _ => await GetGameUrlLiveDealerAsync(brandMerchant, playerCode, cultureCode).ConfigureAwait(false)
        };

        private async ValueTask<BigGamingResponse> GetGameUrlFishingAsync(BigGamingBrandMerchant brandMerchant, string playerCode, string externalId, string cultureCode)
        {
            var request = new BigGamingGetGameUrlFishingRequest()
            {
                Random = GuidExtensions.NewSequentialGuid().ToString(),
                Sn = brandMerchant.SecretCode,
                LoginId = playerCode,
                Locale = cultureCode,
                IsMobileUrl = 0,
                IsHttpsUrl = 0,
                ReturnUrl = string.Empty,
                FromIp = string.Empty,
                GameType = externalId
            };
            return await _client.GetGameUrlAsync(brandMerchant.ApiUrl, brandMerchant.SecretCode, brandMerchant.SecretKey, request).ConfigureAwait(false);
        }

        private async ValueTask<BigGamingResponse> GetGameUrlLiveDealerAsync(BigGamingBrandMerchant brandMerchant, string playerCode, string cultureCode)
        {
            var request = new BigGamingGetGameUrlLiveDealerRequest()
            {
                Random = GuidExtensions.NewSequentialGuid().ToString(),
                Sn = brandMerchant.SecretCode,
                LoginId = playerCode,
                Locale = cultureCode,
                IsMobileUrl = 0,
                IsHttpsUrl = 0,
                ReturnUrl = string.Empty,
                FromIp = string.Empty,
            };
            return await _client.GetGameUrlAsync(brandMerchant.ApiUrl, brandMerchant.SecretCode, brandMerchant.AgentLoginIdPassword, request).ConfigureAwait(false);
        }
    }
}
