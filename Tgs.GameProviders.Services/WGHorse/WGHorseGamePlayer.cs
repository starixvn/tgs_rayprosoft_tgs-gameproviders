using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.WGHorse;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Services.WGHorse
{
    internal class WGHorseGamePlayer : IWGHorseGamePlayer
    {
        private readonly IWGHorseApiClient _client;
        private readonly ILogger<WGHorseGamePlayer> _logger;
        private readonly IWGHorseService _service;

        public WGHorseGamePlayer(
            ILogger<WGHorseGamePlayer> logger,
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

            var token = await _client.GetPrepareLoginAsync(
                configuration.ApiUrl,
                configuration.PartnerCode,
                configuration.PartnerKey,
                configuration.IV,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                playerCode,
                playerCode,
                gameProviderCurrencyCode).ConfigureAwait(false);
            return !string.IsNullOrEmpty(token);
        }
    }
}
