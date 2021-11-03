using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.SBO;
using TGS.GameProviders.Core.SBO.Models;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.SBO
{
    public class SBOGamePlayer : ISBOGamePlayer
    {
        private readonly ISBOApiClient _client;
        private readonly ILogger<SBOGamePlayer> _logger;

        public SBOGamePlayer(
            ILogger<SBOGamePlayer> logger,
            ISBOApiClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client;
        }

        public async ValueTask<bool> CreatePlayerAsync(
            SBOConfiguration configuration,
            string brandCode,
            string playerCode,
            string currencyCode)
        {
            var request = new SBORegisterPlayerRequest(configuration.AgentPrefix, brandCode, currencyCode)
            {
                CompanyKey = configuration.CompanyKey,
                PlayerCode = playerCode
            };

            var response = await _client.RegisterPlayerAsync(configuration.ApiUrl, request).ConfigureAwait(false);

            return response.Error.Code == SBOApiErrorCode.None;
        }
    }
}
