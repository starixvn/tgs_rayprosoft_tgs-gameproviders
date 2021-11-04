using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.WGSports;
using TGS.GameProviders.Core.WGSports.Models;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.WGSports
{
    public class WGSportsGamePlayer : IWGSportsGamePlayer
    {
        private readonly IWGSportsApiClient _client;
        private readonly ILogger<WGSportsGamePlayer> _logger;
        private readonly IWGSportsService _service;

        public WGSportsGamePlayer(
            ILogger<WGSportsGamePlayer> logger,
            IWGSportsApiClient client,
            IWGSportsService service
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<bool> CreatePlayerAsync(WGSportsConfiguration configuration, string playerCode, string currencyCode)
        {
            var gameProviderCurrencyCode = await _service.GetGameProviderCurrencyCodeAsync(currencyCode);
            var prepareLoginRequest = new WGSportsPrepareLoginRequest()
            {
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                PlayerCode = playerCode,
                PlayerNickname = playerCode,
                CurrencyCode = gameProviderCurrencyCode,
            };
            var prepareLoginResponse = await _client.GetPrepareLoginAsync(
                configuration.ApiUrl,
                configuration.PartnerCode,
                configuration.PartnerKey,
                configuration.IV,
                prepareLoginRequest).ConfigureAwait(false);
            return (prepareLoginResponse.ReturnCode == 1);
        }
    }
}
