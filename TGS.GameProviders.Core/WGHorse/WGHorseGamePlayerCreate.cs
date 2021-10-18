using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.WGHorse.Models;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.WGHorse
{
    internal class WGHorseGamePlayerCreate : IWGHorseGamePlayerCreate
    {
        private readonly IWGHorseApiClient _client;
        private readonly ILogger<WGHorseGamePlayerCreate> _logger;
        private readonly IWGHorseService _service;

        public WGHorseGamePlayerCreate(
            ILogger<WGHorseGamePlayerCreate> logger,
            IWGHorseApiClient client,
            IWGHorseService service
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<bool> CreatePlayerAsync(WGHorseConfiguration configuration, string playerCode, string currencyCode)
        {
            var gameProviderCurrencyCode = await _service.GetGameProviderCurrencyCodeAsync(currencyCode);
            var prepareLoginRequest = new WGHorsePrepareLoginRequest()
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
