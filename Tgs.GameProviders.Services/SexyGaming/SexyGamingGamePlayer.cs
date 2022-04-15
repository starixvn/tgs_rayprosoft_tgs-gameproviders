using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.Extensions;
using Tgs.GameProviders.Core.SexyGaming;
using Tgs.GameProviders.Core.SexyGaming.Models;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Services.SexyGaming
{
    internal class SexyGamingGamePlayer : ISexyGamingGamePlayer
    {
        private readonly ISexyGamingApiClient _client;
        private readonly ILogger<SexyGamingGamePlayer> _logger;
        private readonly ISexyGamingService _service;

        public SexyGamingGamePlayer(
            ILogger<SexyGamingGamePlayer> logger,
            ISexyGamingApiClient client,
            ISexyGamingService service
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<bool> CreatePlayerAsync(SexyGamingConfiguration configuration, string playerCode, string currencyCode, string gameProviderCurrency, string gameProviderCulture)
        {
            var BetLime = configuration.BetLimit.SexybcrtMapper
                       .Where(x => x.Value == currencyCode).Select(x => x.Key).ToArray();

            var createMemberRequest = new SexyGamingCreateMemberRequest
            {
                AgentId = configuration.AgentId,
                Cert = configuration.Cert,
                UserId = playerCode,
                Currency = gameProviderCurrency,
                BetLimit = JsonSerializer.Serialize(new
                {
                    SEXYBCRT = new
                    {
                        LIVE = new
                        {
                            limitId = BetLime
                        }
                    }
                }),
                Language = gameProviderCulture
            };
            var response = await _client.CreatePlayerAsync(configuration.ApiUrl, createMemberRequest).ConfigureAwait(false);

            return response.Status.IgnoreCaseEquals(((int)SexyGamingGameApiError.Success).ToString("d4"));
        }
    }
}
