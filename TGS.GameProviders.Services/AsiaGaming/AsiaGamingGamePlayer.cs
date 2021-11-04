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
        private static readonly string _Separator = "/\\\\/";
        private readonly IAsiaGamingApiClient _client;
        private readonly ILogger<AsiaGamingGamePlayer> _logger;
        private readonly IAsiaGamingUtility _utility;

        public AsiaGamingGamePlayer(
            ILogger<AsiaGamingGamePlayer> logger,
            IAsiaGamingUtility utility,
            IAsiaGamingApiClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
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

            var parameter = _utility.GetQueryString(request, _Separator);

            var encodedParas = _utility.DesEncrypt(parameter, configuration.DesKey);
            var key = _utility.Md5Hash($"{encodedParas}{configuration.Md5Key}");

            var response = await _client
                .CheckOrCreateAccountAsync(configuration.LoginApiUrl, encodedParas, key, request)
                .ConfigureAwait(false);

            return response.Result == "0";
        }
    }
}