using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.VR.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.VR.ApplicationServices;

public class VRPlayerFundCheckingGetService : IGetService<VRPlayerFundCheckingQuery, bool>
{
    private readonly IVRApiClient _client;
    private readonly IVRService _service;

    public VRPlayerFundCheckingGetService(
        IVRApiClient client,
        IVRService service)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask<bool> GetAsync(VRPlayerFundCheckingQuery command, CancellationToken cancellationToken = default)
    {
        var brandMerchant = await _service.GetBrandMerchantAsync(command.BrandCode).ConfigureAwait(false)
            ?? throw new Exception("平台商務不存在");

        var data = new VRFundCheckingRequest
        {
            StartTime = command.StartTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            EndTime = command.EndTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            SerialNumber = command.OrderId,
            PlayerCode = command.PlayerCode,
            RecordPage = 0,
            RecordCountPerPage = 1
        };

        var response = await _client.FundCheckingAsync(
            brandMerchant.ApiUrl,
            brandMerchant.Merchantid,
            brandMerchant.Version,
            brandMerchant.Deskey,
            data)
            .ConfigureAwait(false);

        return response != null && response?.TotalRecords == 1;
    }
}
