using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.SBO;
using TGS.GameProviders.Core.SBO.Models;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.SBO
{
    internal class SBOGameAgent : ISBOGameAgent
    {
        private readonly ISBOApiClient _client;
        private readonly ILogger<SBOGameAgent> _logger;
        private readonly ISBOService _service;

        public SBOGameAgent(
            ILogger<SBOGameAgent> logger,
            ISBOService service,
            ISBOApiClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async ValueTask<bool> CreateAgentAsync(
            SBOConfiguration configuration,
            string agentId,
            string currencyCode)
        {
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

            var gameProviderCurrencyCode = await _service
                .GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode)
                .ConfigureAwait(false);

            var request = new SBOCreateAgentRequest
            {
                AgentId = agentId,
                CurrencyCode = gameProviderCurrencyCode,
                CompanyKey = configuration.CompanyKey
            };

            var response = await _client.CreateAgentAsync(configuration.ApiUrl, request)
                .ConfigureAwait(false);

            return response.Error.Code == SBOApiErrorCode.None;
        }
    }
}