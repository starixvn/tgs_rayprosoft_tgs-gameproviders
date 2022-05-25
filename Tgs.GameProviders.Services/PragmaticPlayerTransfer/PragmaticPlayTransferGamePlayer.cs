using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.PragmaticPlayTransfer;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Services.PragmaticPlayerTransfer;

internal class PragmaticPlayTransferGamePlayer : IPragmaticPlayTransferGamePlayer
{
    private readonly IPragmaticPlayTransferApiClient _client;

    public PragmaticPlayTransferGamePlayer(
        IPragmaticPlayTransferApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public ValueTask<bool> CreatePlayerAsync(
        string playerCode,
        string gameProviderCurrencyCode,
        PragmaticPlayTransferConfiguration configuration)
        => _client.CreatePlayerAsync(
            configuration.ApiUrl,
            configuration.SecureLogin,
            configuration.SecretKey,
            playerCode,
            gameProviderCurrencyCode);
}