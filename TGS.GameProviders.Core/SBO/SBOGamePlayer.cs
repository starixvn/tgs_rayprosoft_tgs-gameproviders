using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.SBO.Models;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;
using static TGS.GameProviders.ApiClients.SBO.Models.SBOApiErrorCode;

namespace TGS.GameProviders.Core.SBO
{
    public class SBOGamePlayer : ISBOGamePlayer
    {
        private readonly ISBOGameAgent _agentService;
        private readonly ISBOApiClient _client;
        private readonly ILogger<SBOGamePlayer> _logger;
        private readonly ISBOUtility _utility;

        public SBOGamePlayer(
            ILogger<SBOGamePlayer> logger,
            ISBOUtility utility,
            ISBOGameAgent agentService,
            ISBOApiClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
            _agentService = agentService ?? throw new ArgumentNullException(nameof(agentService));
            _client = client;
        }

        public async ValueTask<bool> CreatePlayerAsync(
            SBOConfiguration configuration,
            string brandCode,
            string playerCode,
            string currencyCode)
        {
            var agentId = _utility.GetAgentId(configuration.AgentPrefix, brandCode, currencyCode);

            var request = new SBOCreatePlayerRequest
            {
                CompanyKey = configuration.CompanyKey,
                PlayerCode = playerCode,
                AgentId = agentId
            };

            var response = await _client.CreatePlayerAsync(configuration.ApiUrl, request).ConfigureAwait(false);

            if (response.Error.Code == None)
            {
                return true;
            }

            if (response.Error.Code != AgentNotFound)
            {
                return false;
            }

            var created = await _agentService.CreateAgentAsync(configuration, agentId, currencyCode)
                .ConfigureAwait(false);

            if (!created)
            {
                return false;
            }

            response = await _client.CreatePlayerAsync(configuration.ApiUrl, request).ConfigureAwait(false);

            return response.Error.Code == None;
        }
    }
}