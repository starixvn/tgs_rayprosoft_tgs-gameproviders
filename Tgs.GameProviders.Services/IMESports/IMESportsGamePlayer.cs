using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.IMESports;
using Tgs.GameProviders.Core.IMESports.Models;
using Tgs.GameProviders.Domains.SettingConfiguration.IMESports;

namespace Tgs.GameProviders.Services.IMESports;

internal class IMESportsGamePlayer : IIMESportsGamePlayer
{
    private readonly IIMESportsApiClient _client;
    private readonly ILogger<IMESportsGamePlayer> _logger;

    public IMESportsGamePlayer(
        ILogger<IMESportsGamePlayer> logger,
        IIMESportsApiClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<bool> CreatePlayerAsync(
        IMESportsBrandMerchant merchant,
        string imesPlayerId,
        string gameProviderCurrencyCode)
    {
        var request = new IMESportsRegisterPlayerRequest
        {
            MerchantCode = merchant.MerchantCode,
            IMESPlayerId = imesPlayerId,
            CurrencyCode = gameProviderCurrencyCode
        };

        var response = await _client.RegisterPlayerAsync(merchant.ApiUrl, request)
            .ConfigureAwait(false);

        return response.ErrorCode == IMESportsGameApiError.Success;
    }
}
