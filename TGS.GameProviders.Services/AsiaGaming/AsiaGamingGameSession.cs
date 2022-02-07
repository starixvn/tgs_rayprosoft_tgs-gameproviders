using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.AsiaGaming;
using TGS.GameProviders.Core.AsiaGaming.Models;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.AsiaGaming
{
    internal class AsiaGamingGameSession : IAsiaGamingGameSession
    {
        private readonly IAsiaGamingApiClient _client;
        private readonly ILogger<AsiaGamingGameSession> _logger;

        public AsiaGamingGameSession(
            ILogger<AsiaGamingGameSession> logger,
            IAsiaGamingApiClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async ValueTask<bool> CreateGameSessionAsync(
            AsiaGamingConfiguration configuration,
            string playerCode,
            Guid loginRecordId,
            decimal playerBalance)
        {
            var request = new AsiaGamingCreateGameSessionRequest
            {
                ProductId = configuration.ProductionId,
                PlayerCode = playerCode,
                LoginRecordId = loginRecordId,
                Balance = playerBalance
            };

            var response = await _client.CreateGameSessionAsync(configuration.ApiUrl, request).ConfigureAwait(false);

            return response.Error == "OK";
        }
    }
}
