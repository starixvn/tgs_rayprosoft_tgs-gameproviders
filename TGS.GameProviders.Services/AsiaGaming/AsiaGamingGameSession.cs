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
        private readonly IAsiaGamingService _service;
        private readonly IAsiaGamingUtility _utility;

        public AsiaGamingGameSession(
            ILogger<AsiaGamingGameSession> logger,
            IAsiaGamingService service,
            IAsiaGamingUtility utility,
            IAsiaGamingApiClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async ValueTask<bool> CreateGameSessionAsync(AsiaGamingConfiguration configuration, string playerCode,
            Guid loginRecordId)
        {
            var balance = await _service.GetPlayerBalanceAsync(playerCode).ConfigureAwait(false);

            if (!balance.HasValue)
            {
                return false;
            }

            var request = new AsiaGamingCreateGameSessionRequest
            {
                ProductId = configuration.ProductionId,
                PlayerCode = playerCode,
                LoginRecordId = loginRecordId,
                Balance = balance.Value
            };

            var response = await _client.CreateGameSessionAsync(configuration.ApiUrl, request).ConfigureAwait(false);

            return response.Error == "OK";
        }
    }
}
