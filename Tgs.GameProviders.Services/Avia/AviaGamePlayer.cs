using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.Avia;
using Tgs.GameProviders.Domains.SettingConfiguration.Avia;

namespace Tgs.GameProviders.Services.Avia;

internal class AviaGamePlayer : IAviaGamePlayerService
{
    private readonly IAviaApiClient _client;

    public AviaGamePlayer(IAviaApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public ValueTask<bool> CreatePlayerAsync(AviaBrandMerchant brandMerchant, string playerCode, string currencyCode)
        => _client.RegisterAsync(
            brandMerchant.ApiUrl,
            brandMerchant.Authorization,
            playerCode,
            brandMerchant.PlayerPassword,
            currencyCode);
}
