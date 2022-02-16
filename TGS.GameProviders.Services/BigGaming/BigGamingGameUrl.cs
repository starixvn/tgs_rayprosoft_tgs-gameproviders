using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.QueryServer.Models;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.BigGaming;
using TGS.GameProviders.Core.BigGaming.Models;
using TGS.GameProviders.Core.BigGaming.Models.Commons;
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Domains.SettingConfiguration.BigGaming;

namespace TGS.GameProviders.Services.BigGaming
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

        public async ValueTask<BigGamingResponse> GetGameUrlAsync(BigGamingBrandMerchant brandMerchant, string playerCode, GameInfo game, string cultureCode)
            => game.GameType
            switch
            {
                GameType.Fishing => await GetGameUrlFishingAsync(brandMerchant, playerCode, game.GameExternalId, cultureCode).ConfigureAwait(false),
                _ => await GetGameUrlLiveDealerAsync(brandMerchant, playerCode, cultureCode).ConfigureAwait(false)
            };

        private async ValueTask<BigGamingResponse> GetGameUrlFishingAsync(BigGamingBrandMerchant brandMerchant, string playerCode, string externalId, string cultureCode)
        {
            var request = new BigGamingGetGameUrlFishingRequest()
            {
                Random = GuidExtensions.NewSequentialGuid().ToString(),
                Sn = brandMerchant.SecretCode,
                LoginId = playerCode,
                Lang = cultureCode.ToLower() switch
                {
                    "zh_cn" => "zh-CN",
                    "vi_vn" => "vi-VN",
                    "th_th" => "th-TH",
                    _ => "en-US",
                },
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
