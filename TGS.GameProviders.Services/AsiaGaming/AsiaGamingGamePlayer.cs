using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.AsiaGaming;
using TGS.GameProviders.Core.AsiaGaming.Models;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.AsiaGaming
{
    internal class AsiaGamingGamePlayer : IAsiaGamingGamePlayer
    {
        private readonly IAsiaGamingApiClient _client;
        private readonly ILogger<AsiaGamingGamePlayer> _logger;

        public AsiaGamingGamePlayer(
            ILogger<AsiaGamingGamePlayer> logger,
            IAsiaGamingApiClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async ValueTask<bool> CheckOrCreateAccountAsync(
            AsiaGamingConfiguration configuration,
            string playerCode,
            string gameProviderCurrencyCode)
        {
            var request = new AsiaGamingCheckOrCreateAccountRequest
            {
                AgentCode = configuration.AgentCode,
                PlayerCode = playerCode,
                Currency = gameProviderCurrencyCode
            };

            var response = await _client
                .CheckOrCreateAccountAsync(
                    configuration.LoginApiUrl,
                    configuration.Md5Key,
                    configuration.DesKey,
                    request)
                .ConfigureAwait(false);

            return response.Result == "0";
        }
    }
}