using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;

internal class PragmaticPlayTransferPlayerFundOutGetService : IGetService<PragmaticPlayTransferPlayerFundOutQuery, PlayerFundResult>
{
    private readonly IPragmaticPlayTransferService _service;
    private readonly IPragmaticPlayTransferApiClient _client;

    public PragmaticPlayTransferPlayerFundOutGetService(
        IPragmaticPlayTransferService service,
        IPragmaticPlayTransferApiClient client)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<PlayerFundResult> GetAsync(
        PragmaticPlayTransferPlayerFundOutQuery query,
        CancellationToken cancellationToken = default)
    {
        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        if (configuration is null)
        {
            throw new Exception($"The {GameProviderType.PragmaticPlayTransfer} configuration does not exist.");
        }

        var success = await _client.FundOutAsync(
            configuration.ApiUrl,
            configuration.SecureLogin,
            configuration.SecretKey,
            query.PlayerCode,
            query.OrderId,
            query.Amount).ConfigureAwait(false);

        return success ?
            new PlayerFundResult
            {
                OrderId = query.OrderId
            } : null;
    }
}