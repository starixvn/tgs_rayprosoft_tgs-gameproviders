using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.SkywindService.Client;
using Tgs.GameInterface.SkywindService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SkywindTransfer.ApplicationServices;

public class SkywindTransferPlayerFundCheckingGetService : IGetService<SkywindTransferPlayerFundCheckingQuery, bool>
{
    private readonly IGameInterfaceSkywindClient _client;
    private readonly ISettingRepository _setting;

    public SkywindTransferPlayerFundCheckingGetService(
        IGameInterfaceSkywindClient client,
        ISettingRepository setting)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _setting = setting ?? throw new ArgumentNullException(nameof(setting));
    }

    public async ValueTask<bool> GetAsync(SkywindTransferPlayerFundCheckingQuery query, CancellationToken cancellationToken = default)
    {
        var configuration = await _setting.GetSkywindTransferConfigurationAsync().ConfigureAwait(false);

        var request = new FundStatusRequest()
        {
            ApiUrl = configuration.ApiUrl.ToString(),
            SecretKey = configuration.SecretKey,
            Username = configuration.Username,
            Password = configuration.Password,
            ExtTrxId = query.OrderId
        };

        var reply = await _client
                .FundStatusAsync(
                    request: request,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

        return !reply.IsTransaction
            ? throw new Exception($"This order does not exist.({query.OrderId})")
            : reply.Status.Equals("committed", StringComparison.OrdinalIgnoreCase);
    }
}
