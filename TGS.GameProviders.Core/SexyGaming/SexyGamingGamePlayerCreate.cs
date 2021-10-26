using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.SexyGaming.Models;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;
using System.Text.Json;
using TGS.GameProviders.ApiClients.SexyGaming;
using TGS.GameProviders.Domains.Extensions;

namespace TGS.GameProviders.Core.SexyGaming
{
    internal class SexyGamingGamePlayerCreate : ISexyGamingGamePlayerCreate
    {
        private readonly ISexyGamingApiClient _client;
        private readonly ILogger<SexyGamingGamePlayerCreate> _logger;
        private readonly ISexyGamingService _service;

        public SexyGamingGamePlayerCreate(
            ILogger<SexyGamingGamePlayerCreate> logger,
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
