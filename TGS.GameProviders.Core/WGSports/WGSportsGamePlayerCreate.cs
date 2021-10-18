using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.WGSports.Models;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.WGSports
{
    public class WGSportsGamePlayerCreate : IWGSportsGamePlayerCreate
    {
        private readonly IWGSportsApiClient _client;
        private readonly ILogger<WGSportsGamePlayerCreate> _logger;
        private readonly IWGSportsService _service;

        public WGSportsGamePlayerCreate(
            ILogger<WGSportsGamePlayerCreate> logger,
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
            var prepareLoginResponse = await _client.GetPrepareLoginAsync(configuration, prepareLoginRequest).ConfigureAwait(false);
            return (prepareLoginResponse.ReturnCode == 1);
        }
    }
}
