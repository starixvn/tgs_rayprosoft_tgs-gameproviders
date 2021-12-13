using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Avia;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.Avia
{
    internal class AviaGamePlayer : IAviaGamePlayerService
    {
        private readonly IAviaApiClient _client;

        public AviaGamePlayer(IAviaApiClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public ValueTask<bool> CreatePlayerAsync(AviaConfiguration configuration, string playerCode, string currencyCode)
            => _client.RegisterAsync(
                configuration.ApiUrl,
                configuration.Authorization,
                playerCode,
                configuration.PlayerPassword,
                currencyCode);
    }
}
