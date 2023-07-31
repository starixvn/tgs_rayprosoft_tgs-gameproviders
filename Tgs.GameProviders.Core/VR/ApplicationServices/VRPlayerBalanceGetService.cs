using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.VR.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.VR.ApplicationServices;

public class VRPlayerBalanceGetService : IGetService<VRPlayerBalanceQuery, PlayerBalanceResult>
{
    private readonly IVRApiClient _client;
    private readonly IVRGamePlayer _playerCreate;
    private readonly IPlayerRepository _playerService;
    private readonly IVRService _service;

    public VRPlayerBalanceGetService(
        IVRApiClient client,
        IVRGamePlayer playerCreate,
        IPlayerRepository playerService,
        IVRService service
        )
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _playerCreate = playerCreate ?? throw new ArgumentNullException(nameof(playerCreate));
        _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask<PlayerBalanceResult> GetAsync(VRPlayerBalanceQuery command, CancellationToken cancellationToken = default)
    {
        var playerModel = await _playerService
            .GetAsync(
            playerCode: command.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var brandMerchant = await _service.GetBrandMerchantAsync(playerModel.BrandCode).ConfigureAwait(false);

        if (brandMerchant == null)
        {
            return null;
        }

        var data = new VRGetBalanceRequest()
        {
            PlayerCode = command.PlayerCode
        };

        var response = await _client.GetBalanceAsync(
            brandMerchant.ApiUrl,
            brandMerchant.Merchantid,
            brandMerchant.Version,
            brandMerchant.Deskey,
            data).ConfigureAwait(false);

        if (string.IsNullOrEmpty(response.PlayerName) || response.Balance == -1)
        {
            if (await _playerCreate.CreatePlayerAsync(brandMerchant, playerModel.PlayerCode).ConfigureAwait(false))
            {
                response = await _client.GetBalanceAsync(
                    brandMerchant.ApiUrl,
                    brandMerchant.Merchantid,
                    brandMerchant.Version,
                    brandMerchant.Deskey,
                    data).ConfigureAwait(false);
            }
        }
        return (string.IsNullOrEmpty(response.PlayerName) || response.Balance == -1)
            ? throw new Exception($"Player does not exist.{command.PlayerCode}")
            : new PlayerBalanceResult
            {
                FreeBalance = response.Balance
            };
    }
}
