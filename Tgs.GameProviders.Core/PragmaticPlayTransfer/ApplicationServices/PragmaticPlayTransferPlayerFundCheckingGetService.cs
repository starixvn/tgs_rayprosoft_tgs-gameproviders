using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;

internal class PragmaticPlayTransferPlayerFundCheckingGetService
    : IGetService<PragmaticPlayTransferPlayerFundCheckingQuery, PlayerFundCheckingResult>
{
    private readonly IPragmaticPlayTransferService _service;
    private readonly IPragmaticPlayTransferApiClient _client;

    public PragmaticPlayTransferPlayerFundCheckingGetService(
        IPragmaticPlayTransferService service,
        IPragmaticPlayTransferApiClient client)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<PlayerFundCheckingResult> GetAsync(
        PragmaticPlayTransferPlayerFundCheckingQuery query,
        CancellationToken cancellationToken = default)
    {
        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        if (configuration is null)
        {
            throw new Exception($"The {GameProviderType.PragmaticPlayTransfer} configuration does not exist.");
        }

        var isExist = await _client
            .FundOrderCheckingAsync(
                configuration.ApiUrl,
                configuration.SecureLogin,
                configuration.SecretKey,
                query.OrderId)
            .ConfigureAwait(false);

        return new PlayerFundCheckingResult
        {
            OrderStatus = isExist
        };
    }
}